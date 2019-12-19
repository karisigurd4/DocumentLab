namespace DocumentLab.PageInterpreter.Components
{
  using Antlr4.Runtime;
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.Contracts.PageInterpreter;
  using DocumentLab.PageInterpreter.Grammar;
  using DocumentLab.PageInterpreter.Interfaces;
  using DocumentLab.PageInterpreter.Interpreter;
  using System;

  public class Interpreter : IInterpreter
  {
    private string script;

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
