namespace DocumentLab.ImageProcessor.Interfaces
{
  using System.Drawing;
  using System.Collections.Generic;

  public interface IImageAnalyzer
  {
    IEnumerable<Point[]> GetContours(IEnumerable<byte> image, int width, int height);
  }
}
