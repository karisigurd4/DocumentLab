namespace DocumentLab.PageInterpreter.Components
{
  using Antlr4.Runtime;
  using Antlr4.Runtime.Misc;
  using DocumentLab.PageInterpreter.Exceptions;
  using System;

  public class ErrorListener : BaseErrorListener
  {
    public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
    {
      throw new SyntaxException($"Line: {line}, Char: {charPositionInLine} on value '{offendingSymbol.Text}' - {msg}");
    }
  }
}