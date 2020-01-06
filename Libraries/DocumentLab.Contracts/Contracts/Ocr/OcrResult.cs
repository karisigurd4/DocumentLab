namespace DocumentLab.Contracts.Ocr
{
  using System.Drawing;

  public class OcrResult 
  {
    public Rectangle ContourOffset { get; set; }
    public string[] Result { get; set; }
    public Rectangle BoundingBox { get; set; }
  }
}
