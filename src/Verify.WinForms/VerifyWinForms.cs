using System.ComponentModel;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

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
        // Verify uses the first registered converter that matches, so the more specific types
        // must be registered before Control.
        VerifierSettings.RegisterFileConverter<Form>(FormToImage);
        VerifierSettings.RegisterFileConverter<ToolStripDropDown>(DropDownToImage);
        VerifierSettings.RegisterFileConverter<UserControl>(ControlToImage);
        VerifierSettings.RegisterFileConverter<Control>(ControlToImage);
    }

    static readonly Point offScreen = new(-20000, -20000);

    // A drop-down (ContextMenuStrip, a menu item's or drop-down button's DropDown, etc) is a popup
    // window of its own. Show it as one, off screen, so it is laid out and drawn (renderer,
    // margins, border) as it appears on screen, then close it again.
    static ConversionResult DropDownToImage(ToolStripDropDown dropDown, IReadOnlyDictionary<string, object> context) =>
        Convert(() =>
        {
            dropDown.Opening += MoveOffScreen;
            try
            {
                // Showing at the off screen point, not just Show(), also stops WinForms moving the
                // window back to the point it was last shown at once it is visible.
                dropDown.Show(offScreen);
            }
            finally
            {
                dropDown.Opening -= MoveOffScreen;
            }

            try
            {
                Application.DoEvents();
                return ControlToStream(dropDown);
            }
            finally
            {
                dropDown.Close();
            }
        });

    // WinForms keeps a top-level drop-down on screen whatever point it is shown at, and places a
    // drop-down that belongs to an item by that item. So move the window itself off screen once
    // WinForms has placed it but before it is made visible: it never appears on screen, and the
    // mouse cannot be over it to highlight an item.
    static void MoveOffScreen(object? sender, CancelEventArgs e)
    {
        if (e.Cancel)
        {
            return;
        }

        var dropDown = (ToolStripDropDown) sender!;
        SetWindowPos(dropDown.Handle, IntPtr.Zero, offScreen.X, offScreen.Y, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
    }

    const uint SWP_NOSIZE = 0x0001;
    const uint SWP_NOZORDER = 0x0004;
    const uint SWP_NOACTIVATE = 0x0010;

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);

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
