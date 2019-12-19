namespace DocumentLab.Contracts
{
  using System;

  /// <summary>
  /// Used to avoid including System.Drawing in the interpreter but still retaining boundingbox information
  /// </summary>
  public class PageUnitBoundingBox : ICloneable, IEquatable<PageUnitBoundingBox>
  {
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public PageUnitBoundingBox()
    {
    }

    public PageUnitBoundingBox(int x, int y, int width, int height)
    {
      this.X = x;
      this.Y = y;
      this.Width = width;
      this.Height = height;
    }

    public object Clone()
    {
      return new PageUnitBoundingBox()
      {
        Height = this.Height,
        Width = this.Width,
        X = this.X,
        Y = this.Y
      };
    }

    public bool Equals(PageUnitBoundingBox other)
    {
      return this.X == other.X
        && this.Y == other.Y
        && this.Width == other.Width
        && this.Height == other.Height;
    }
  }
}
