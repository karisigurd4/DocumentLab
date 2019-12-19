namespace DocumentLab.Contracts.Decorators.Ocr

{
  using System.Collections.Generic;
  using System.Drawing;

  public class BitmapWithOcrMetaInfo
  {
    public IEnumerable<byte> BitmapAsBytes { get; set; }
    public Rectangle OcrBoundingBox { get; set; }
  }
}
