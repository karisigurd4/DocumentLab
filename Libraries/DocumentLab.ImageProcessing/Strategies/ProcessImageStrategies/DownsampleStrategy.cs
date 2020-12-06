namespace DocumentLab.ImageProcessor.Strategies.ProcessImageStrategies
{
  using Interfaces;
  using ImageMagick;
  using System.Linq;
  using System.Collections.Generic;

  public class DownsampleStrategy : IProcessImageStrategy
  {
    public IEnumerable<byte> PreProcess(IEnumerable<byte> bitmap)
    {
      using (var processed = new MagickImage(bitmap.ToArray()))
      {
        processed.Resize(new Percentage(Constants.ScaleDownPercentage));
        return processed.ToByteArray();
      }
    }
  }
}
