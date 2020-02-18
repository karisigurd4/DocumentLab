namespace DocumentLab.Interfaces
{
  using System.Drawing;

  public interface IDocumentInterpreter
  {
    string InterpretToJson(string script, Bitmap bitmap);
  }
}
