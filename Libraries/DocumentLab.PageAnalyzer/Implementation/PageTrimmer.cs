namespace DocumentLab.PageAnalyzer.Implementation
{
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.Contracts.PageInterpreter;
  using DocumentLab.PageAnalyzer.Interfaces;
  using System;
  using System.Linq;
  using System.Collections.Generic;

  public class PageTrimmer : IPageTrimmer
  {
    public Page MovePageUnit(Page page, Coordinate from, Coordinate to)
    {
      var pageUnits = page.Contents[from.X, from.Y];

      foreach (var pageUnit in pageUnits)
      {
        pageUnit.Coordinate = (Coordinate)to.Clone();

        if (page.Contents[to.X, to.Y] == null)
          page.Contents[to.X, to.Y] = new List<PageUnit>();

        page.Contents[to.X, to.Y].Add(pageUnit);
      }

      page.Contents[from.X, from.Y] = null;

      return page;
    }

    public Page TrimPage(Page page, int? trimX = null, int? trimY = null, bool clone = false)
    {
      if(clone)
      {
        page = (Page)page.Clone();
      }

      trimX = trimX == null ? Convert.ToInt32(Constants.PageAnalyzerConfiguration["TrimX"]) : trimX;
      trimY = trimY == null ? Convert.ToInt32(Constants.PageAnalyzerConfiguration["TrimY"]) : trimY;

      for (int x = 0; x < page.Contents.GetLength(0); x++)
      {
        for (int y = 0; y < page.Contents.GetLength(1); y++)
        {
          var currentPageUnit = page.Contents?[x, y]?.FirstOrDefault();
          if (currentPageUnit == null)
            continue;

          var surroundingColumns = Enumerable.Range(x == 0 ? 0 : -1, 1).Where(r => r != 0).SelectMany(c => 
            Enumerable.Range(0, page.Contents.GetLength(1))
              .Select(e => page.Contents?[c + x, e]))
              .ToArray();

          var surroundingRows = Enumerable.Range(y == 0 ? 0 : -1, 1).Where(r => r != 0).SelectMany(r =>
            Enumerable.Range(0, page.Contents.GetLength(0))
              .Select(e => page.Contents?[e, r + y]))
              .ToArray();

          var toColumnPageUnit = surroundingColumns
            .Where(p => p != null && p.Count > 0 && p.Min(c => Math.Abs(currentPageUnit.BoundingBox.X - c.BoundingBox.X)) < trimX)
            .FirstOrDefault(); ;

          var toRowPageUnit = surroundingRows
            .Where(p => p != null && p.Count > 0 && p.Min(c => Math.Abs(currentPageUnit.BoundingBox.Y - c.BoundingBox.Y)) < trimY)
            .FirstOrDefault();

          if(toColumnPageUnit != null || toRowPageUnit != null)
          {
            MovePageUnit(page, currentPageUnit.Coordinate, new Coordinate()
            {
              X = toColumnPageUnit != null ? toColumnPageUnit.FirstOrDefault().Coordinate.X : currentPageUnit.Coordinate.X,
              Y = toRowPageUnit != null ? toRowPageUnit.FirstOrDefault().Coordinate.Y : currentPageUnit.Coordinate.Y
            });
          }
        }
      }

      return page;
    } 
  }
}
