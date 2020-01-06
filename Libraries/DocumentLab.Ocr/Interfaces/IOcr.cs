namespace DocumentLab.Ocr.Interfaces
{
  using Contracts.Ocr;
  using System.Drawing;

  public interface IOcr
  {
    OcrResult[] PerformOcr(Bitmap highResImage, Bitmap lowResImage = null);
  }
}
