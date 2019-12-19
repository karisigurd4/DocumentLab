namespace DocumentLab.Ocr.Interfaces
{
  using DocumentLab.Contracts;
  using System.Drawing;

  public interface IOcr
  {
    OcrResult[] PerformOcr(Bitmap highResImage, Bitmap lowResImage = null);
  }
}
