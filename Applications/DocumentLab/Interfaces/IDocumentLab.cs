namespace DocumentLab.Interfaces
{
  using System.Drawing;

  public interface IDocumentLab
  {
    string InterpretToJson(string script, Bitmap bitmap);
  }
}
