namespace DocumenLab
{
  using DocumentLab.Contracts;
  using DocumentLab.TextAnalyzer.Utilities;
  using global::DocumentLab;
  using global::DocumentLab.Interfaces;
  using global::DocumentLab.PageInterpreter;
  using System.Drawing;

  public class DocumentInterpreter : DocumentLabBase, IDocumentInterpreter
  {
    public DocumentInterpreter() : base() { }

    public string InterpretToJson(Bitmap bitmap, string script)
      => InterpretToJson(AnalyzePage(bitmap), script);

    public string InterpretToJson(Page analyzedPage, string script) 
      => documentInterpreter.Interpret(analyzedPage, script).ConvertToJson(script);

    public Page AnalyzePage(Bitmap bitmap)
      => GetAnalyzedPage(bitmap);

    public string[] GetDefinedTextTypes()
      => TextTypeHelper.GetDefinedTextTypes();
  }
}
