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
        SetWindowPos(dropDown.Handle, IntPtr.Zero, offScreen.X, offScreen.Y, 0, 0, noSize | noZorder | noActive);
    }

    const uint noSize = 0x0001;
    const uint noZorder = 0x0004;
    const uint noActive = 0x0010;

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);

    // A form is shown as the top-level window it is, off screen and without being activated, so it
    // is laid out, scaled and drawn as it appears on screen, and its Load and Shown handlers (and
    // anything else it posts while being shown) have run. It is hidden again after. A form the
    // caller has already shown is drawn where it is, and left shown.
    static ConversionResult FormToImage(Form form, IReadOnlyDictionary<string, object> context) =>
        Convert(() =>
        {
            if (form.Visible)
            {
                Application.DoEvents();
                return ControlToStream(form);
            }

            form.ShowInTaskbar = false;
            form.StartPosition = FormStartPosition.Manual;
            form.Location = offScreen;
            ShowWithoutActivating(form);
            try
            {
                Application.DoEvents();
                return ControlToStream(form);
            }
            finally
            {
                form.Hide();
            }
        });

    // Showing a form activates it, which takes the foreground from whatever window is in use, even
    // from a test run in the background, and only the form itself can opt out (by overriding
    // ShowWithoutActivation). So veto its activation with a hook on this thread while it is shown.
    static void ShowWithoutActivating(Form form)
    {
        HookProc hook = (code, wParam, lParam) =>
        {
            if (code == cbtActivate &&
                form.IsHandleCreated &&
                wParam == form.Handle)
            {
                // ReSharper disable once RedundantCast
                return (IntPtr) 1;
            }

            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        };
        var hookHandle = SetWindowsHookEx(cbtHook, hook, IntPtr.Zero, GetCurrentThreadId());
        try
        {
            form.Show();
        }
        finally
        {
            UnhookWindowsHookEx(hookHandle);
            GC.KeepAlive(hook);
        }
    }

    const int cbtHook = 5;
    const int cbtActivate = 5;

    delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    static extern IntPtr SetWindowsHookEx(int hookType, HookProc hook, IntPtr module, uint threadId);

    [DllImport("user32.dll")]
    static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

    [DllImport("user32.dll")]
    static extern IntPtr CallNextHookEx(IntPtr hookHandle, int code, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    static extern uint GetCurrentThreadId();

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
