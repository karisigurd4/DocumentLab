namespace DocumentLab.ImageProcessor.Implementation
{
  using Extensions;
  using Contracts.Enums.Operations;
  using Interfaces;
  using Emgu.CV;
  using Emgu.CV.CvEnum;
  using Emgu.CV.Util;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.Collections.Generic;

  public class ImageAnalyzer : IImageAnalyzer
  {
    private readonly IImageProcessor imageProcessor;

    public ImageAnalyzer(IImageProcessor imageProcessor)
    {
      this.imageProcessor = imageProcessor;
    }

    public IEnumerable<Point[]> GetContours(IEnumerable<byte> lowResImage, int width, int height)
    {
      using (var preProcessedHiglight = imageProcessor.Process(ProcessImageOperation.PreProcessForHighlight, lowResImage))
      {
        using (var highlightTextAreas = imageProcessor.Process(ProcessImageOperation.HighlightTextAreas, preProcessedHiglight.ToByteArray(ImageFormat.Png)))
        {
          using (var scaledLowResImage = new Bitmap(highlightTextAreas, new Size(width, height)))
          {
            return FindContours(scaledLowResImage);
          }
        }
      } 
    }

    private static IEnumerable<Point[]> FindContours(Bitmap image)
    {
      using (var cvImage = image.ToCvImage())
      {
        var contour = new VectorOfVectorOfPoint();

        CvInvoke.CvtColor(cvImage, cvImage, ColorConversion.Rgb2Gray);
        CvInvoke.FindContours(cvImage, contour, new Mat(), RetrType.External, ChainApproxMethod.ChainApproxTc89Kcos);

        return contour.ToArrayOfArray();
      }
    }
  }
}
