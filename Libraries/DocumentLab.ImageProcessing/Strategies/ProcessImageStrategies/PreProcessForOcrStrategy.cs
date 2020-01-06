namespace DocumentLab.ImageProcessor.Strategies.PreProcessStrategies
{
  using Interfaces;
  using ImageMagick;
  using System.Linq;
  using System.Collections.Generic;

  public class PreProcessForOcrStrategy : IProcessImageStrategy
  {
    public IEnumerable<byte> PreProcess(IEnumerable<byte> bitmap)
    {
      using (var processed = new MagickImage(bitmap.ToArray()))
      {
        processed.Deskew(new Percentage(60));
        return processed.ToByteArray();
      }
    }
  }
}
