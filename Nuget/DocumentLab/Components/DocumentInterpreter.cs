namespace DocumenLab
{
  using DocumentLab.Contracts;
  using global::DocumentLab;
  using global::DocumentLab.Interfaces;
  using global::DocumentLab.PageInterpreter;
  using System.Drawing;

  public class DocumentInterpreter : DocumentLabBase, IDocumentInterpreter
  {
    public DocumentInterpreter() : base() { }

    public string InterpretToJson(string script, Bitmap bitmap)
    {
      return InterpretToJson(AnalyzePage(bitmap), script);
    }

    public Page AnalyzePage(Bitmap bitmap)
    {
      return GetAnalyzedPage(bitmap);
    }

    public string InterpretToJson(Page analyzedPage, string script)
    {
      return documentInterpreter.Interpret(analyzedPage, script).ConvertToJson(script);
    }
  }
}
