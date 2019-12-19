namespace DocumentLab.PageInterpreter.Interfaces
{
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.Contracts.PageInterpreter;

  public interface IInterpreter
  {
    InterpreterResult Interpret(Page page, string script);
  }
}
