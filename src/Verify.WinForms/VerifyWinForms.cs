using System.Drawing.Imaging;

namespace VerifyTests;

public static class VerifyWinForms
{
    public static bool Initialized { get; private set; }

    public static void Initialize()
    {
        if (Initialized)
        {
            throw new("Already Initialized");
        }

        Initialized = true;
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        VerifierSettings.RegisterFileConverter<Form>(FormToImage);
        VerifierSettings.RegisterFileConverter<ContextMenuStrip>(MenuToImage);
        VerifierSettings.RegisterFileConverter<UserControl>(ControlToImage);
        VerifierSettings.RegisterFileConverter<Control>(ControlToImage);
    }

    static ConversionResult MenuToImage(ContextMenuStrip control, IReadOnlyDictionary<string, object> context) =>
        Convert(() =>
        {
            using var form = new Form
            {
                Width = control.Width,
                Height = control.Height,
                ContextMenuStrip = control,
                ShowInTaskbar = false,
                TopLevel = false,
                AutoScaleMode = AutoScaleMode.None
            };
            control.TopLevel = false;
            control.Show();
            return ControlToStream(control);
        });

    static ConversionResult FormToImage(Form form, IReadOnlyDictionary<string, object> context) =>
        Convert(() => FormToStream(form));

    static ConversionResult ControlToImage(Control control, IReadOnlyDictionary<string, object> context) =>
        Convert(() =>
        {
            using var form = new Form
            {
                Width = control.Width,
                Height = control.Height,
                ShowInTaskbar = false,
                TopLevel = false,
                AutoScaleMode = AutoScaleMode.None
            };
            form.Controls.Add(control);
            form.Show();
            return ControlToStream(control);
        });

    static Stream FormToStream(Form form)
    {
        form.ShowInTaskbar = false;
        form.TopLevel = false;
        form.AutoScaleMode = AutoScaleMode.None;
        form.Show();
        return ControlToStream(form);
    }

    static Stream ControlToStream(Control control)
    {
        using var bitmap = new Bitmap(control.Width, control.Height, PixelFormat.Format32bppArgb);
        control.DrawToBitmap(bitmap, new(0, 0, control.Width, control.Height));
        var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream;
    }

    // Showing a WinForms control installs a WindowsFormsSynchronizationContext as
    // SynchronizationContext.Current on the current thread. Verify runs its pipeline free of any
    // SynchronizationContext (it does async IO without ConfigureAwait(false); see
    // SettingsTask.ToTask), so restore the previous context after rendering. Otherwise the leaked
    // context would be captured by Verify's downstream async IO and its continuation posted to a
    // message pump that is never run - deadlocking whenever a snapshot is new or mismatched.
    static ConversionResult Convert(Func<Stream> render)
    {
        var syncContext = SynchronizationContext.Current;
        try
        {
            return new(null, "png", render());
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(syncContext);
        }
    }
}
