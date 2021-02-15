using System.Collections.Generic;
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
            VerifierSettings.RegisterFileConverter<Form>(FormToImage);
            VerifierSettings.RegisterFileConverter<ContextMenuStrip>(MenuToImage);
            VerifierSettings.RegisterFileConverter<UserControl>(ControlToImage);
            VerifierSettings.RegisterFileConverter<Control>(ControlToImage);
        }

        static ConversionResult MenuToImage(ContextMenuStrip control, IReadOnlyDictionary<string, object> context)
        {
            using Form form = new()
            {
                Width = control.Width,
                Height = control.Height,
                ContextMenuStrip = control,
                ShowInTaskbar = false
            };
            form.Show();
            control.Show();
            return new(null, "png", ControlToImage(control));
        }

        static ConversionResult FormToImage(Form form, IReadOnlyDictionary<string, object> context)
        {
            return new(null, "png", FormToStream(form));
        }

        static ConversionResult ControlToImage(Control control, IReadOnlyDictionary<string, object> context)
        {
            using Form form = new()
            {
                Width = control.Width,
                Height = control.Height
            };
            form.Controls.Add(control);
            form.ShowInTaskbar = false;
            form.Show();
            return new(null, "png", ControlToImage(control));
        }

        static Stream FormToStream(Form form)
        {
            form.ShowInTaskbar = false;
            form.Show();
            return ControlToImage(form);
        }

        static Stream ControlToImage(Control control)
        {
            using Bitmap bitmap = new(control.Width, control.Height, PixelFormat.Format32bppArgb);
            control.DrawToBitmap(bitmap, new(0, 0, control.Width, control.Height));
            MemoryStream stream = new();
            bitmap.Save(stream, ImageFormat.Png);
            return stream;
        }
    }
}