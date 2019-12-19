namespace DocumentLab.ImageProcessor.Interfaces
{
  using DocumentLab.Contracts.Contracts.ImageProcessor;
  using DocumentLab.Contracts.Enums.Operations;
  using System.Collections.Generic;
  using System.Drawing;

  public interface IImageProcessor
  {
    Bitmap Process(ProcessImageOperation type, IEnumerable<byte> bitmap);
    ImageWithCoordinates SplitByPoint(IEnumerable<byte> bitmap, Point[] point);
    TrimInformation GetTrimmedBoundingBox(IEnumerable<byte> image, Rectangle boundingBox);
  }
}
