namespace DocumentLab.Ocr.Interfaces
{
  using Contracts.Ocr;
  using Contracts.Decorators.Ocr;
  using System.Collections.Generic;

  public interface ITesseractPool
  {
    IEnumerable<OcrResult> PerformOcr(IEnumerable<BitmapWithOcrMetaInfo> image);
    IEnumerable<OcrResult> PerformOcr(BitmapWithOcrMetaInfo image);
  }
}
