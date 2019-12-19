namespace DocumentLab.PageInterpreter.Interfaces
{
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.Contracts.PageInterpreter;
  using DocumentLab.Contracts.Enums.Types;
  using System;
  using System.Collections.Generic;

  public interface IPageTraverser : ICloneable
  {
    bool ErrorOccurred { get; }
    PageUnit CurrentPageUnit { get; }
    PageUnit[] CurrentPageUnits { get; }

    TraversalResult Traverse(Direction direction);
    Page GetPage();
    PageIndex GetCurrentPosition();
    PageUnit GetMatchingPageUnit(string textType);
    IEnumerable<PageUnit> Peek(Direction direction, int steps = 1);
  }
}
