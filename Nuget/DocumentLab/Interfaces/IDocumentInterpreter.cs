namespace DocumentLab.Interfaces
{
  using DocumentLab.Contracts;
  using System.Drawing;

  public interface IDocumentInterpreter
  {
    string InterpretToJson(Bitmap bitmap, string script);
    string InterpretToJson(Page analyzedPage, string script);
    Page AnalyzePage(Bitmap bitmap);
    string[] GetDefinedTextTypes();
  }
}
