namespace DocumentLab.ImageProcessor.Interfaces
{
  using DocumentLab.Contracts.Contracts.ImageProcessor;
  using System.Collections.Generic;
  using System.Drawing;

  public interface IImageAnalyzer
  {
    IEnumerable<Point[]> GetContours(IEnumerable<byte> image, int width, int height);
  }
}
