namespace DocumentLab
{
  using System.Drawing;
  using Interfaces;
  using PageInterpreter;

  public class DocumentLab : DocumentLabBase, IDocumentLab
  {
    public DocumentLab() : base() { }

    public string InterpretToJson(string script, Bitmap bitmap)
    {
      var page = GetAnalyzedPage(bitmap);

      return documentInterpreter.Interpret(page, script).ConvertToJson(script);
    }
  }
}
