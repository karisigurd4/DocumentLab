namespace DocumentLab.Ocr.Implementation
{
  using Contracts.Ocr;
  using Contracts.Decorators.Ocr;
  using Utils;
  using Tesseract;
  using ImageProcessor.Extensions;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;

  public class TesseractEngineWrapper : IDisposable
  {
    public TesseractEngine Engine { get; set; }
    public int NumberOfAwaiters { get; set; } = 0;

    private readonly Semaphore engineLock = new Semaphore(1, 1);

    public TesseractEngineWrapper(string languageFilePath = null, string language = null)
    {
      Engine = new TesseractEngine(
        Path.Combine(languageFilePath ?? AppDomain.CurrentDomain.BaseDirectory, language ?? Constants.LanguageFilePath),
        Constants.DefaultLanguage,
        EngineMode.Default);
    }

    public IEnumerable<OcrResult> ProcessOcrResult(BitmapWithOcrMetaInfo image)
    {
      NumberOfAwaiters += 1;
      engineLock.WaitOne();

      Thread.CurrentThread.Priority = ThreadPriority.Highest;

      IEnumerable<OcrResult> outResults = null;
      using (var page = Engine.Process(image.BitmapAsBytes.ToBitmap(), PageSegMode.SingleBlock))
      {
        var rawText = OcrUtils.RemoveOcrArtifacts(page.GetHOCRText(0, false));

        var ocrWords = OcrUtils
          .GetWords(rawText, image.OcrBoundingBox);

        outResults = ocrWords
          .Select(x => new OcrResult()
          {
            ContourOffset = image.OcrBoundingBox,
            BoundingBox = x.Key,
            Result = new string[] { x.Value }
          });
      }

      Thread.CurrentThread.Priority = ThreadPriority.Normal;

      engineLock.Release();
      NumberOfAwaiters -= 1;

      return outResults;
    }

    public void Dispose()
    {
      Engine.Dispose();
    }
  }
}
