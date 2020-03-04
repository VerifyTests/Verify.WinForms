using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Verify
{
    public static class VerifyWinForms
    {
        public static void Enable()
        {
            SharedVerifySettings.RegisterFileConverter<Form>("png", FormToImage);
            SharedVerifySettings.RegisterFileConverter<Control>("png", FormToImage);
        }

        static ConversionResult FormToImage(Form form, VerifySettings settings)
        {
            return new ConversionResult(null, FormToStream(form));
        }

        static ConversionResult FormToImage(Control control, VerifySettings settings)
        {
            using var form = new Form
            {
                Width = control.Width,
                Height = control.Height
            };
            form.Controls.Add(control);
            form.ShowInTaskbar = false;
            form.Show();
            return new ConversionResult(null, ControlToImage(control));
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