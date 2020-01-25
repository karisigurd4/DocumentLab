namespace DocumentLab.PageInterpreter.Components
{
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.PageInterpreter;
  using DocumentLab.Core.Utils;
  using DocumentLab.PageAnalyzer.Implementation;
  using DocumentLab.PageInterpreter.DataModel;
  using DocumentLab.PageInterpreter.Interfaces;
  using System;
  using System.Linq;

  public class TableAnalyzer : ITableAnalyzer
  {
    public InterpreterResult AnalyzeTable(Page page, TableColumn[] tableColumns)
    {
      var tableAnalysisPage = new PageTrimmer().TrimPage(page, 0, 20, true);
      
      // This finds the row in the page that can best match the TextType and label definitions along with the indices where they match. 
      var bestMatch = tableColumns
        .SelectMany((tableColumn, TableIndex) => tableAnalysisPage
          .GetIndexWhere(t => tableColumn.LabelParameters.Any(v => FuzzyTextComparer.FuzzyEquals(v, t.Value)))
          .Select(PageIndex => new
          {
            TableIndex,
            PageIndex,
            tableColumn.TextType
          }))
        .GroupBy(x => x.Index.Coordinate.Y)
        .OrderByDescending(x => x.Count())
        .FirstOrDefault();

      throw new NotImplementedException();
    }
  }
}
