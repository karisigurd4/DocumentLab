namespace DocumenLab
{
  using global::DocumentLab;
  using global::DocumentLab.Interfaces;
  using global::DocumentLab.PageInterpreter;
  using System.Drawing;

  public class DocumentInterpreter : DocumentLabBase, IDocumentInterpreter
  {
    public DocumentInterpreter() : base() { }

    public string InterpretToJson(string script, Bitmap bitmap)
    {
      var page = GetAnalyzedPage(bitmap);

      return documentInterpreter.Interpret(page, script).ConvertToJson(script);
    }
  }
}
