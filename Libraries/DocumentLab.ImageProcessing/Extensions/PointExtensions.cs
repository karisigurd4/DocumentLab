namespace DocumentLab.ImageProcessor.Extensions
{
  using System.Drawing;
  using System.Linq;

  public static class PointExtensions
  {
    public static Rectangle GetBoundingBox(this Point[] pointArray, int offset = 0)
    {
      return new Rectangle(
        pointArray.Min(x => x.X) - offset,
        pointArray.Min(x => x.Y) - offset,
        pointArray.Max(x => x.X) - pointArray.Min(x => x.X) + offset,
        pointArray.Max(x => x.Y) - pointArray.Min(x => x.Y) + offset);
    }
  }
}
