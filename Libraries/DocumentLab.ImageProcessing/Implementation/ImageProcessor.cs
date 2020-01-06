namespace DocumentLab.ImageProcessor.Implementation
{
  using Contracts.ImageProcessor;
  using Core.Interfaces;
  using Contracts.Enums.Operations;
  using Extensions;
  using Interfaces;
  using ImageMagick;
  using System;
  using System.Drawing;
  using System.Collections.Generic;
  using System.Drawing.Imaging;

  public class ImageProcessor : IImageProcessor
  {
    private readonly IStrategyFactoryBuilder strategyFactoryBuilder;

    public ImageProcessor(IStrategyFactoryBuilder strategyFactoryBuilder)
    {
      this.strategyFactoryBuilder = strategyFactoryBuilder;
    }

    public Bitmap Process(ProcessImageOperation operation, IEnumerable<byte> bitmap) => strategyFactoryBuilder
        .BuildStrategyFactory<IProcessImageStrategy>(operation)
        .PreProcess(bitmap)
        .ToBitmap();

    public ImageWithCoordinates SplitByPoint(IEnumerable<byte> bitmap, Point[] point)
    {
      var asBitmap = bitmap.ToBitmap();
      var boundingBox = point.GetBoundingBox(10).AdjustForBitmapSize(asBitmap);
      var splitBitmap = asBitmap.Clone(boundingBox, asBitmap.PixelFormat);

      var trimAnalysis = GetTrimmedBoundingBox(splitBitmap.ToByteArray(ImageFormat.Png), boundingBox);

      return new ImageWithCoordinates()
      {
        BoundingBox = trimAnalysis.BoundingBox,
        Image = splitBitmap.ToByteArray(ImageFormat.Png)
      };
    }

    public TrimInformation GetTrimmedBoundingBox(IEnumerable<byte> image, Rectangle boundingBox)
    {
      using (var processedImage = new MagickImage(image.ToBitmap()))
      {
        processedImage.ColorSpace = ColorSpace.Gray;
        processedImage.ColorType = ColorType.Grayscale;
        processedImage.BackgroundColor = MagickColor.FromRgb(255, 255, 255);
        processedImage.ColorFuzz = new Percentage(10);
        var formatResult = processedImage.FormatExpression("%@");

        var formatInfo = formatResult.Split('+');
        int trimmedFromLeft = Convert.ToInt32(formatInfo[1]);
        int trimmedFromTop = Convert.ToInt32(formatInfo[2]);

        boundingBox.X += trimmedFromLeft;
        boundingBox.Y += trimmedFromTop;

        return new TrimInformation()
        {
          BoundingBox = boundingBox
        };
      }
    }
  }
}
