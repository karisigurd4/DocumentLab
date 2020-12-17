namespace DocumentLab.Interfaces
{
  using DocumentLab.Contracts;
  using System.Drawing;

  public interface IDocumentInterpreter
  {
    string InterpretToJson(string script, Bitmap bitmap);
    Page AnalyzePage(Bitmap bitmap);
    string InterpretToJson(Page analyzedPage, string script);
  }
}
