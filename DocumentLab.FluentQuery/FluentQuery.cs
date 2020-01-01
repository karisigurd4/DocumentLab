namespace DocumentLab
{
  using DocumentLab.Contracts;
  using PageInterpreter.Interfaces;

  public class FluentQuery
  {
    public string Script { get; set; }
    public Page AnalyzedPage { get; set; }
    public IInterpreter Interpreter { get; set; }
    public QueryType QueryType { get; set; } = QueryType.None;
  }
}
