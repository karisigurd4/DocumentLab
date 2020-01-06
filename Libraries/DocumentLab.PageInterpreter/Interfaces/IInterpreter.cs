namespace DocumentLab.PageInterpreter.Interfaces
{
  using Contracts;
  using Contracts.PageInterpreter;

  public interface IInterpreter
  {
    InterpreterResult Interpret(Page page, string script);
  }
}
