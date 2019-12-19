namespace DocumentLab.ImageProcessor.Strategies.ProcessImageStrategies
{
  using DocumentLab.ImageProcessor.Interfaces;
  using ImageMagick;
  using System.Collections.Generic;
  using System.Linq;

  public class DownsampleStrategy : IProcessImageStrategy
  {
    public IEnumerable<byte> PreProcess(IEnumerable<byte> bitmap)
    {
      using (var processed = new MagickImage(bitmap.ToArray()))
      {
        processed.Resize(new Percentage(25));
        return processed.ToByteArray();
      }
    }
  }
}
