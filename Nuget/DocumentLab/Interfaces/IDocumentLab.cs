namespace DocumentLab.Interfaces
{
  using global::DocumentLab.Contracts.Contracts.PageInterpreter;
  using System.Drawing;

  public interface IDocumentLab
  {
    InterpreterResult Interpret(string script, Bitmap bitmap);
  }
}
