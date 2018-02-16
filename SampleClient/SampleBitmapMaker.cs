using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestPubSub
{
    internal class SampleBitmapMaker
    {
        internal static BitmapSource CreateMissingBitmap(string captionText)
        {
            var visual = new DrawingVisual();
            var dc = visual.RenderOpen();
            var whitePen = new Pen(Brushes.White, 1);
            dc.DrawRectangle(Brushes.White, whitePen, new Rect(new Size(400, 400)));
            var redPen = new Pen(Brushes.Red, 5);
            //dc.DrawLine(redPen, new Point(12, 9), new Point(627, 470));
            //dc.DrawLine(redPen, new Point(627, 9), new Point(12, 470));
            dc.DrawEllipse(Brushes.Bisque, redPen, new Point(200, 200), 50, 64);
            dc.DrawText(new FormattedText(captionText,
                            CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Tahoma"), 32, Brushes.RoyalBlue), new Point(12, 9));
            dc.Close();
            var bmp = new RenderTargetBitmap(400, 400, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(visual);
            return bmp;
        }
    }
    internal class SampleBitmapSerializer
    {
        internal static string GetSerializedBitmap(string captionText)
        {
            var img = SampleBitmapMaker.CreateMissingBitmap(captionText);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.QualityLevel = 100;
            string s = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(img));
                encoder.Save(stream);
                byte[] bit = stream.ToArray();

                s = JsonConvert.SerializeObject(bit);
            }
            return s;
        }

        internal static BitmapImage DeserializeBitmap(string serial)
        {
            BitmapImage bi = new BitmapImage();
            var img = JsonConvert.DeserializeObject<byte[]>(serial);
            using (MemoryStream stream = new MemoryStream(img))
            {
                var bitmap = new System.Drawing.Bitmap(stream);
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                stream.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = stream;
                bi.EndInit();
                bi.Freeze();
            }
            return bi;
        }
    }
}
