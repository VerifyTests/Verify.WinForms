using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

public class WinFormsUtils
{
    public static string ScreenCapture(string received, Control controlUnderTest)
    {
        using (var hidden = new Form())
        {
            EnsureControlDisplaysCorrectlyByAddingItToAHiddenForm(hidden, controlUnderTest);
            SavePng(received, controlUnderTest);
        }
        return received;
    }

    public static void SavePng(string received, Control control)
    {
        using var bitmap = new Bitmap(control.Width, control.Height, PixelFormat.Format32bppArgb);
        control.DrawToBitmap(bitmap, new Rectangle(0, 0, control.Width, control.Height));
        bitmap.Save(received, ImageFormat.Png);
    }

    public static void ConfigureInnerDisplay(Form hidden, Control controlUnderTest)
    {
        AddToParent(hidden, controlUnderTest);

        hidden.ShowInTaskbar = false;
        hidden.AllowTransparency = true;
        hidden.Opacity = 0;

        hidden.Show();
        controlUnderTest.Show();
    }

    public static void EnsureControlDisplaysCorrectlyByAddingItToAHiddenForm(Form hidden, Control controlUnderTest)
    {
        AddToParent(hidden, controlUnderTest);

        hidden.ShowInTaskbar = false;
        hidden.AllowTransparency = true;
        hidden.Opacity = 0;

        hidden.Show();
        controlUnderTest.Show();
    }

    static void AddToParent(Form hidden, Control controlUnderTest)
    {
        if (controlUnderTest is Form form)
        {
            hidden.IsMdiContainer = true;
            form.MdiParent = hidden;
        }
        else
        {
            hidden.Controls.Add(controlUnderTest);
        }
    }
}