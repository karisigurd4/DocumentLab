namespace DocumentLab.ImageProcessor.Strategies.ProcessImageStrategies
{
  using DocumentLab.ImageProcessor.Interfaces;
  using ImageMagick;
  using System.Collections.Generic;
  using System.Linq;

  public class HighlightTextAreasStrategy : IProcessImageStrategy
  {
    public IEnumerable<byte> PreProcess(IEnumerable<byte> bitmap)
    {
      using (var processed = new MagickImage(bitmap.ToArray()))
      {
        processed.ContrastStretch(new Percentage(0));

        processed.Morphology(MorphologyMethod.Dilate, Kernel.Diamond, Constants.HighlightIntensity);

        processed.Threshold(new Percentage(20));

        return processed.ToByteArray();
      }
    }
  }
}
