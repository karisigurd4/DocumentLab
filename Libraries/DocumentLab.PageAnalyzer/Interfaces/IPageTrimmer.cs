namespace DocumentLab.PageAnalyzer.Interfaces
{
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.Contracts.PageInterpreter;
  using System;

  public interface IPageTrimmer
  {
    Page TrimPage(Page page, int? trimX = null, int? trimY = null, bool clone = false);
    Page MovePageUnit(Page page, Coordinate from, Coordinate to);
    
  }
}
