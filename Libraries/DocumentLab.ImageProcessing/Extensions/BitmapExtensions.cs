namespace DocumentLab.ImageProcessor.Extensions
{
  using Emgu.CV;
  using Emgu.CV.Structure;
  using System.Collections.Generic;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.IO;
  using System.Linq;

  public static class BitmapExtensions
  {
    public static Image<Bgr, byte> ToCvImage(this Bitmap bitmap)
    {
      return new Image<Bgr, byte>(bitmap);
    }

    public static IEnumerable<byte> ToByteArray(this Image image, ImageFormat format)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        image.Save(ms, ImageFormat.Png);
        return ms.ToArray();
      }
    }

    public static Bitmap ToBitmap(this IEnumerable<byte> byteArray)
    {
      using (var ms = new MemoryStream(byteArray.ToArray()))
      {
        return new Bitmap(ms);
      }
    }
  }
}
