namespace DocumentLab.Ocr.Implementation
{
  using Interfaces;
  using System.Drawing;
  using System.Linq;
  using Core.Extensions;
  using DocumentLab.ImageProcessor.Interfaces;
  using DocumentLab.ImageProcessor.Extensions;
  using DocumentLab.Contracts.Enums.Operations;
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.Decorators.Ocr;
  using System.Diagnostics;

  public class Ocr : IOcr
  {
    private readonly IImageAnalyzer documentAnalyzer;
    private readonly IImageProcessor imageProcessor;
    private readonly ITesseractPool tesseractWrapper;

    public Ocr(IImageAnalyzer documentAnalyzer, IImageProcessor imageProcessor, ITesseractPool tesseractWrapper)
    {
      this.documentAnalyzer = documentAnalyzer;
      this.imageProcessor = imageProcessor;
      this.tesseractWrapper = tesseractWrapper;
    }

    public OcrResult[] PerformOcr(Bitmap highResImage, Bitmap lowResImage = null)
    {
      Stopwatch sw = new Stopwatch();
      sw.Start();

      var highResAsBytes = highResImage.ToByteArray(Constants.ConvertBetween);
      var lowResAsBytes = lowResImage?.ToByteArray(Constants.ConvertBetween);

      var documentParts = documentAnalyzer.GetContours(lowResAsBytes, highResImage.Width, highResImage.Height)
        .AsParallel()
        .WithDegreeOfParallelism(Constants.NumberOfThreads)
        .WithMergeOptions(ParallelMergeOptions.NotBuffered)
        .Select(contour => imageProcessor.SplitByPoint(highResAsBytes, contour))
        .Select(x => new BitmapWithOcrMetaInfo()
        {
          BitmapAsBytes = imageProcessor.Process(ProcessImageOperation.EnhanceForOcr, x.Image).ToByteArray(Constants.ConvertBetween),
          OcrBoundingBox = x.BoundingBox
        });

      Debug.WriteLine(sw.ElapsedMilliseconds);
      sw.Restart();

      var result = documentParts
        .ToList()
        .SplitList(Constants.ResultChunkSize)
        .AsParallel()
        .WithDegreeOfParallelism(Constants.NumberOfThreads)
        .WithMergeOptions(ParallelMergeOptions.NotBuffered)
        .SelectMany(x => tesseractWrapper.PerformOcr(x));

      Debug.WriteLine(sw.ElapsedMilliseconds);
      sw.Stop();

      return result.ToArray();
    }
  }
}
