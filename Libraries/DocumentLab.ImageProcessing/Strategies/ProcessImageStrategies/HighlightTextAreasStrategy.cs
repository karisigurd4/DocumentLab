namespace DocumentLab.ImageProcessor.Strategies.ProcessImageStrategies
{
  using Interfaces;
  using ImageMagick;
  using System.Linq;
  using System.Collections.Generic;

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
