namespace DocumentLab.PageInterpreterUnitTest
{
  using Antlr4.Runtime;
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.PageInterpreter;
  using DocumentLab.PageInterpreter.Grammar;
  using DocumentLab.PageInterpreter.Components;
  using DocumentLab.PageInterpreter.Interpreter;

  public class InterpreterTestBase
  {
    public InterpreterResult Visit(string query, Page page)
    {
      var patternInterpreterLexer = new PageInterpreterLexer(new AntlrInputStream(query));
      var patternInterpreterParser = new PageInterpreterParser(new CommonTokenStream(patternInterpreterLexer));
      patternInterpreterParser.AddErrorListener(new ErrorListener());

      var visitor = new PageInterpreter(page);

      visitor.Visit(patternInterpreterParser.compileUnit());
      return visitor.Result;
    }
  }
}
