namespace DocumentLab.Contracts.PageInterpreter
{
  using System;

  public class PageIndex : ICloneable
  {
    public Coordinate Coordinate { get; set; }
    public int PageUnitIndex { get; set; }

    public PageIndex()
    {
    }

    public PageIndex(int coordinateX, int coordinateY, int pageUnitIndex = 0)
    {
      this.Coordinate = new Coordinate(coordinateX, coordinateY);
      this.PageUnitIndex = pageUnitIndex;
    }

    public object Clone()
    {
      return new PageIndex()
      {
        Coordinate = (Coordinate)this.Coordinate.Clone(),
        PageUnitIndex = this.PageUnitIndex
      };
    }
  }
}
