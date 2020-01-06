namespace DocumentLab
{
  using Contracts;
  using PageInterpreter.Interfaces;

  public class FluentQuery
  {
    public string Script { get; set; }
    
    public FluentQuery(Page analyzedPage, IInterpreter interpreter)
    {
      this.AnalyzedPage = analyzedPage;
      this.Interpreter = interpreter;
    }

    internal Page AnalyzedPage { get; set; }
    internal IInterpreter Interpreter { get; set; }
    internal QueryType QueryType { get; set; } = QueryType.None;
  }
}
