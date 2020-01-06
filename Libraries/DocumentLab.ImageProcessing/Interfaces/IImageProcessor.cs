namespace DocumentLab.ImageProcessor.Interfaces
{
  using Contracts.ImageProcessor;
  using Contracts.Enums.Operations;
  using System.Drawing;
  using System.Collections.Generic;

  public interface IImageProcessor
  {
    Bitmap Process(ProcessImageOperation type, IEnumerable<byte> bitmap);
    ImageWithCoordinates SplitByPoint(IEnumerable<byte> bitmap, Point[] point);
    TrimInformation GetTrimmedBoundingBox(IEnumerable<byte> image, Rectangle boundingBox);
  }
}
