namespace DocumentLab.PageInterpreter.Components
{
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.Contracts.PageInterpreter;
  using DocumentLab.Contracts.Enums.Types;
  using DocumentLab.PageInterpreter.Interfaces;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class PageTraverser : IPageTraverser, ICloneable
  {
    public bool ErrorOccurred { get; private set; }

    public PageUnit CurrentPageUnit => CurrentPageUnits?.Length > pageUnitIndex && pageUnitIndex >= 0 ? CurrentPageUnits?[pageUnitIndex] : null;
    public PageUnit[] CurrentPageUnits => page.Contents[currentPosition.X, currentPosition.Y]?.ToArray(); 

    private readonly Page page;
    private readonly Coordinate currentPosition;

    private int pageUnitIndex = 0;

    public PageTraverser(Page page, PageIndex startIndex)
    {
      this.pageUnitIndex = startIndex.PageUnitIndex;
      this.currentPosition = startIndex.Coordinate;
      this.page = page;
    }

    public Page GetPage()
    {
      return page;
    }

    public TraversalResult Traverse(Direction direction)
    {
      PageUnit current = null;
      int steps = 0;

      while (current == null)
      {
        switch (direction)
        {
          case Direction.Up: TraverseUp(); break;
          case Direction.Down: TraverseDown(); break;
          case Direction.Left: TraverseLeft(); break;
          case Direction.Right: TraverseRight(); break;
          case Direction.NotSet: ErrorOccurred = true; break;
        }

        if (ErrorOccurred)
          return null;

        current = CurrentPageUnit;
        steps++;
      }

      return new TraversalResult()
      {
        PageUnit = current,
        Steps = steps
      };
    }

    public PageIndex GetCurrentPosition()
    {
      return new PageIndex()
      {
        Coordinate = currentPosition,
        PageUnitIndex = pageUnitIndex
      };
    }

    private void TraverseUp()
    {
      if (currentPosition.Y <= 0)
      {
        ErrorOccurred = true;
      }  
      else
      {
        currentPosition.Y -= 1;
        pageUnitIndex = CurrentPageUnits != null ? CurrentPageUnits.Count() - 1 : 0;
      }
    }

    private void TraverseDown()
    {
      if (currentPosition.Y >= page.Contents.GetLength(1) - 1)
      {
        ErrorOccurred = true;
      }
      else
      {
        currentPosition.Y += 1;
        pageUnitIndex = 0;
      }
    }

    private void TraverseLeft()
    {
      if (currentPosition.X <= 0)
      {
        ErrorOccurred = true;
      }
      else
      {
        currentPosition.X -= 1;
        pageUnitIndex = 0;
      }
    }

    private void TraverseRight()
    {
      if (currentPosition.X >= page.Contents.GetLength(0) - 1)
      {
        ErrorOccurred = true;
      }
      else
      {
        currentPosition.X += 1;
        pageUnitIndex = 0;
      }
    }

    public IEnumerable<PageUnit> Peek(Direction direction, int steps = 1)
    {
      int startX = currentPosition.X, startY = currentPosition.Y;
      for (int i = 0; i < steps; i++)
        Traverse(direction);

      var peekPageUnit = GetCurrentPageUnits();

      currentPosition.X = startX;
      currentPosition.Y = startY;
      
      if (ErrorOccurred)
      {
        ErrorOccurred = false;
        return peekPageUnit;
      }

      return peekPageUnit;
    }

    public IEnumerable<PageUnit> GetCurrentPageUnits()
    {
      return page?.Contents?[currentPosition.X, currentPosition.Y] ?? null;
    }

    public PageUnit GetMatchingPageUnit(string textType)
    {
      var current = GetCurrentPageUnits();
      if (current == null)
        return null;

      return current
        .Where(x => x.TextType == textType)
        .FirstOrDefault();
    }

    public object Clone()
    {
      var pageIndex = new PageIndex(currentPosition.X, currentPosition.Y, pageUnitIndex);

      return new PageTraverser(this.page, pageIndex);
    }
  }
}
