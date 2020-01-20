namespace DocumentLab.PageInterpreter.Interfaces
{
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.PageInterpreter;
  using DocumentLab.PageInterpreter.DataModel;

  public interface ITableAnalyzer
  {
    InterpreterResult AnalyzeTable(Page page, TableColumn[] tableColumns);
  }
}
