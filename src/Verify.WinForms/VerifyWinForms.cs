using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace VerifyTests
{
    public static class VerifyWinForms
    {
        public static void Enable()
        {
            VerifierSettings.RegisterFileConverter<Control>(ControlToImage);
            VerifierSettings.RegisterFileConverter<UserControl>(ControlToImage);
            VerifierSettings.RegisterFileConverter<ContextMenuStrip>(MenuToImage);
            VerifierSettings.RegisterFileConverter<Form>(FormToImage);
        }

        static ConversionResult MenuToImage(ContextMenuStrip control, VerifySettings settings)
        {
            using var form = new Form
            {
                Width = control.Width,
                Height = control.Height,
                ContextMenuStrip = control,
                ShowInTaskbar = false
            };
            form.Show();
            control.Show();
            return new ConversionResult(null, "png", ControlToImage(control));
        }

        static ConversionResult FormToImage(Form form, VerifySettings settings)
        {
            return new ConversionResult(null, "png", FormToStream(form));
        }

        static ConversionResult ControlToImage(Control control, VerifySettings settings)
        {
            using var form = new Form
            {
                Width = control.Width,
                Height = control.Height
            };
            form.Controls.Add(control);
            form.ShowInTaskbar = false;
            form.Show();
            return new ConversionResult(null, "png", ControlToImage(control));
        }

        static Stream FormToStream(Form form)
        {
            form.ShowInTaskbar = false;
            form.Show();
            return ControlToImage(form);
        }

        static Stream ControlToImage(Control control)
        {
            using var bitmap = new Bitmap(control.Width, control.Height, PixelFormat.Format32bppArgb);
            control.DrawToBitmap(bitmap, new Rectangle(0, 0, control.Width, control.Height));
            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return stream;
        }
    }
}