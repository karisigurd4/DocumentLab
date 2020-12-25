namespace DocumentLab.PageInterpreter.Components
{
  using Contracts;
  using Contracts.PageInterpreter;
  using Grammar;
  using Interfaces;
  using PageInterpreter.Interpreter;
  using Antlr4.Runtime;
  using System.Linq;
  using System;

  public class Interpreter : IInterpreter
  {
    public InterpreterResult Interpret(Page page, string script)
    {
      var scriptLines = script.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
      for (int i = scriptLines.Count - 1; i >= 0; i--)
      {
        if (scriptLines[i] == string.Empty)
        {
          scriptLines.RemoveAt(i);
        }
        else
        {
          break;
        }
      }

      script = string.Join(Environment.NewLine, scriptLines);

      var patternInterpreterLexer = new PageInterpreterLexer(new AntlrInputStream(script));
      var patternInterpreterParser = new PageInterpreterParser(new CommonTokenStream(patternInterpreterLexer));
      patternInterpreterParser.AddErrorListener(new ErrorListener());

      var visitor = new PageInterpreter(page);

      visitor.Visit(patternInterpreterParser.compileUnit());
      return visitor.Result;
    }
  }
}
