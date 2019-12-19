namespace DocumentLab.Contracts.Utils
{
  using DocumentLab.Contracts.Contracts.PageInterpreter;
  using System.Collections.Generic;
  using System.Drawing;

  public class TestPageCreator
  {
    public Page Page { get; }

    public TestPageCreator(int sizeX, int sizeY)
    {
      Page = new Page()
      {
        Contents = new List<PageUnit>[sizeX, sizeY]
      };
    }

    public void SetValue(int x, int y, string textType, string value, PageUnitBoundingBox boundingBox = null)
    {
      if (Page.Contents[x, y] == null)
        Page.Contents[x, y] = new List<PageUnit>();

      Page.Contents[x, y].Add(new PageUnit()
      {
        BoundingBox = boundingBox,
        Coordinate = new Coordinate()
        {
          X = x,
          Y = y
        },
        TextType = textType,
        Value = value
      });
    }
  }
}
