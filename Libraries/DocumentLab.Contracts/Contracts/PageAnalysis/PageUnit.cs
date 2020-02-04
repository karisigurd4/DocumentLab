namespace DocumentLab.Contracts
{
  using PageInterpreter;
  using System;

  public class PageUnit : ICloneable
  {
    public string Value { get; set; }
    public string TextType { get; set; }
    public PageUnitBoundingBox CountourOffset { get; set; }
    public PageUnitBoundingBox BoundingBox { get; set; }
    public Coordinate Coordinate { get; set; }

    public object Clone()
    {
      return new PageUnit()
      {
        Coordinate = Coordinate,
        TextType = TextType,
        Value = (string)Value.Clone(),
        BoundingBox = (PageUnitBoundingBox)BoundingBox?.Clone(),
        CountourOffset = (PageUnitBoundingBox)CountourOffset?.Clone()
      };
    }
  }
}
