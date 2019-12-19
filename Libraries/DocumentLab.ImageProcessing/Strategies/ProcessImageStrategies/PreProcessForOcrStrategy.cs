namespace DocumentLab.ImageProcessor.Strategies.PreProcessStrategies
{
  using System.Drawing;
  using DocumentLab.ImageProcessor.Interfaces;
  using ImageMagick;
  using System.Collections.Generic;
  using System.Linq;

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
