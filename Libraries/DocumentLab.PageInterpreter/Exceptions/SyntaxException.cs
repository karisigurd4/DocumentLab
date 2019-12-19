namespace DocumentLab.PageInterpreter.Exceptions
{
  using System;

  public class SyntaxException : Exception
  {
    public SyntaxException(string message) : base(message)
    {
    }
  }
}
