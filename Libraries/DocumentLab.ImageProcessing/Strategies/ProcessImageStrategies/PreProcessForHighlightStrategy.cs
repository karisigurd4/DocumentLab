namespace DocumentLab.ImageProcessor.Strategies.PreProcessStrategies
{
  using Interfaces;
  using ImageMagick;
  using System.Linq;
  using System.Collections.Generic;

  public class PreProcessForHighlightStrategy : IProcessImageStrategy
  {
    public IEnumerable<byte> PreProcess(IEnumerable<byte> bitmap)
    {
      using (var processed = new MagickImage(bitmap.ToArray()))
      {
        processed.ColorSpace = ColorSpace.Gray;
        processed.ColorType = ColorType.Grayscale;

        processed.Normalize();

        // Denoise
        using (var denoise = processed.Clone())
        {
          denoise.ColorSpace = ColorSpace.Gray;
          denoise.Negate();
          denoise.AdaptiveThreshold(15, 15, new Percentage(5));
          denoise.ContrastStretch(new Percentage(0));
          processed.Composite(denoise, CompositeOperator.CopyAlpha);
        }

        // Remove lines
        using (var lines = processed.Clone())
        {
          lines.Negate();
          lines.Morphology(MorphologyMethod.Thinning, Kernel.Rectangle, "1x30+0+0^<");
          lines.Negate();
          processed.Composite(lines, CompositeOperator.Lighten);
        }

        processed.Opaque(MagickColors.Transparent, MagickColors.White);
        processed.Alpha(AlphaOption.Off);

        processed.Negate();

        return processed.ToByteArray();
      } 
    }
  }
}
