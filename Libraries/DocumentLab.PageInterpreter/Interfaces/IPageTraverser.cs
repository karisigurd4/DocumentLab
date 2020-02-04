namespace DocumentLab.PageInterpreter.Interfaces
{
  using Contracts;
  using Contracts.PageInterpreter;
  using System;

  public interface IPageTraverser : ICloneable
  {
    bool ErrorOccurred { get; }
    PageUnit CurrentPageUnit { get; }
    PageUnit[] CurrentPageUnits { get; }

    TraversalResult Traverse(Direction direction);
    Page GetPage();
    PageIndex GetCurrentPosition();
    PageUnit GetMatchingPageUnit(string textType);
    PageUnit[] Peek(Direction direction, int steps = 1);
  }
}
