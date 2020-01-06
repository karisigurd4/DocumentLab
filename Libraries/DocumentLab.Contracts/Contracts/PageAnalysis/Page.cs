namespace DocumentLab.Contracts
{
  using System;
  using System.Linq;
  using System.Collections.Generic;
  using PageInterpreter;

  public class Page : ICloneable
  {
    public List<PageUnit>[,] Contents { get; set; }

    public object Clone()
    {
      List<PageUnit>[,] clonedContents = new List<PageUnit>[Contents.GetLength(0), Contents.GetLength(1)];

      for (int x = 0; x < Contents.GetLength(0); x++)
      {
        for (int y = 0; y < Contents.GetLength(1); y++)
        {
          clonedContents[x, y] = new List<PageUnit>();

          if(Contents[x, y] != null)
          {
            clonedContents[x, y].AddRange((IEnumerable<PageUnit>)Contents[x, y].Select(z => (PageUnit)z.Clone()));
          }
        }
      }

      return new Page()
      {
        Contents = clonedContents
      };
    }

    public IEnumerable<PageIndex> GetIndexWhere(Func<PageUnit, bool> predicate)
    {
      List<PageIndex> indices = new List<PageIndex>();

      for (int x = 0; x < Contents.GetLength(0); x++)
      {
        for (int y = 0; y < Contents.GetLength(1); y++)
        {
          if (Contents[x, y] == null)
            continue;

          for (int u = 0; u < Contents[x, y].Count; u++)
          {
            if (predicate(Contents[x, y][u]))
              indices.Add(new PageIndex()
              {
                Coordinate = new Coordinate(x, y),
                PageUnitIndex = u
              });
          }
        }
      }

      return indices;
    }
  }
}
