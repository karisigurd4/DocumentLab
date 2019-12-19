namespace DocumentLab.Ocr.Implementation
{
  using Interfaces;
  using System.Collections.Generic;
  using System.Linq;
  using Contracts;
  using Contracts.Decorators.Ocr;
  using Core.Extensions;

  public class TesseractPool : ITesseractPool
  {
    private static TesseractEngineWrapper[] enginePool = null;

    private TesseractEngineWrapper EngineWithLowestAwaiters
    {
      get
      {
        return enginePool.OrderBy(x => x.NumberOfAwaiters).First();
      }
    }

    public TesseractPool()
    {
      InitializePool();
    }

    public TesseractPool(string languageFilePath = null, string language = null)
    {
      InitializePool(languageFilePath, language);
    }

    private void InitializePool(string languageFilePath = null, string language = null)
    {
      if (enginePool == null)
      {
        enginePool = new TesseractEngineWrapper[Constants.TesseractEnginePoolSize];
        enginePool = enginePool.Select(x => new TesseractEngineWrapper(languageFilePath, language)).ToArray();
      }
    }

    public IEnumerable<OcrResult> PerformOcr(IEnumerable<BitmapWithOcrMetaInfo> images)
    {
      List<OcrResult> pages = new List<OcrResult>();
      images
        .ForEach(x => pages.AddRange(EngineWithLowestAwaiters
        .ProcessOcrResult(x)));
      return pages;
    }

    public IEnumerable<OcrResult> PerformOcr(BitmapWithOcrMetaInfo image)
    {
      return EngineWithLowestAwaiters.ProcessOcrResult(image);
    }
  }
}
