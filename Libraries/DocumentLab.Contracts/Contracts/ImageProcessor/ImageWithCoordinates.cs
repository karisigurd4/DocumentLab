namespace DocumentLab.Contracts.Contracts.ImageProcessor
{
  using System.Collections.Generic;
  using System.Drawing;

  public class ImageWithCoordinates
  {
    public IEnumerable<byte> Image { get; set; }
    public Rectangle BoundingBox { get; set; }
  }
}
