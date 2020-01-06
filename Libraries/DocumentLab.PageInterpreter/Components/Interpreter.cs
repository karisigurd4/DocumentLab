namespace DocumentLab.PageInterpreter.Components
{
  using Contracts;
  using Contracts.PageInterpreter;
  using Grammar;
  using Interfaces;
  using PageInterpreter.Interpreter;
  using Antlr4.Runtime;

  public class Interpreter : IInterpreter
  {
    public InterpreterResult Interpret(Page page, string script)
    {
      var patternInterpreterLexer = new PageInterpreterLexer(new AntlrInputStream(script));
      var patternInterpreterParser = new PageInterpreterParser(new CommonTokenStream(patternInterpreterLexer));
      patternInterpreterParser.AddErrorListener(new ErrorListener());

      var visitor = new PageInterpreter(page);

      visitor.Visit(patternInterpreterParser.compileUnit());
      return visitor.Result;
    }
  }
}
