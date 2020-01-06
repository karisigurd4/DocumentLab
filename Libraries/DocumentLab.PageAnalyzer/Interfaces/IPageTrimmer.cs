namespace DocumentLab.PageAnalyzer.Interfaces
{
  using Contracts;
  using Contracts.PageInterpreter;

  public interface IPageTrimmer
  {
    Page TrimPage(Page page, int? trimX = null, int? trimY = null, bool clone = false);
    Page MovePageUnit(Page page, Coordinate from, Coordinate to);
    
  }
}
