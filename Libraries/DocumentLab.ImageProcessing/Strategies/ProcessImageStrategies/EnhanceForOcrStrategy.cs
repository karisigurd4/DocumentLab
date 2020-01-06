namespace DocumentLab.ImageProcessor.Strategies.PreProcessStrategies
{
  using Interfaces;
  using ImageMagick;
  using System.Linq;
  using System.Collections.Generic;

  public class EnhanceForOcrStrategy : IProcessImageStrategy
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
          denoise.AdaptiveThreshold(10, 10, new Percentage(5));
          denoise.ContrastStretch(new Percentage(0));
          processed.Composite(denoise, CompositeOperator.CopyAlpha);
        }
        processed.Opaque(MagickColors.Transparent, MagickColors.White);
        processed.Alpha(AlphaOption.Off);

        // Deskew
        processed.BackgroundColor = MagickColors.White;
        processed.Deskew(new Percentage(40));

        // Sharpen
        processed.Sharpen(0, 1);

        // Saturate
        processed.Modulate(new Percentage(100), new Percentage(200), new Percentage(100));

        // Trim
        processed.Trim();
        processed.RePage();

        processed.Compose = CompositeOperator.Over;
        processed.BorderColor = MagickColors.White;
        processed.Border(15);

        return processed.ToByteArray();
      }
    }
  }
}
