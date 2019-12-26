namespace DocumentLab.Interfaces
{
  using global::DocumentLab.Contracts.Contracts.PageInterpreter;
  using System.Drawing;

  public interface IDocumentLab
  {
    string InterpretToJson(string script, Bitmap bitmap);
  }
}
