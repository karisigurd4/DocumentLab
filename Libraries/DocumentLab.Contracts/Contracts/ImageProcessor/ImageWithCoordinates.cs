namespace DocumentLab.Contracts.ImageProcessor
{
  using System.Drawing;
  using System.Collections.Generic;

  public class ImageWithCoordinates
  {
    public IEnumerable<byte> Image { get; set; }
    public Rectangle BoundingBox { get; set; }
  }
}
