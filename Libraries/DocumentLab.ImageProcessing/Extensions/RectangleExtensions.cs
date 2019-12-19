namespace DocumentLab.ImageProcessor.Extensions
{
  using System;
  using System.Drawing;

  public static class RectangleExtensions
  {
    public static Rectangle AdjustForBitmapSize(this Rectangle boundingBox, Bitmap bitmap)
    {
      if (boundingBox.Left < 0)
        boundingBox.X = 1;

      if (boundingBox.Right > bitmap.Width)
        boundingBox.Width = bitmap.Width - 10;

      if (boundingBox.Top < 0)
        boundingBox.Y = 1;

      if (boundingBox.Bottom > bitmap.Height)
        boundingBox.Height = bitmap.Height - 10;

      return boundingBox;
    }
  }
}
