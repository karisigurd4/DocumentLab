namespace DocumentLab.PageInterpreter.Components
{
  using Exceptions;
  using Antlr4.Runtime;
  using Antlr4.Runtime.Misc;
  using System;

  public class ErrorListener : BaseErrorListener
  {
    public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
    {
      throw new SyntaxException($"Line: {line}, Char: {charPositionInLine} on value '{offendingSymbol.Text}' - {msg}");
    }
  }
}