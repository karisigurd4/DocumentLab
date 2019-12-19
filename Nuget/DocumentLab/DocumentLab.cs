namespace DocumentLab
{
  using System.Drawing;
  using global::DocumentLab.Contracts.Contracts.PageInterpreter;
  using global::DocumentLab.Contracts.Enums.Operations;
  using global::DocumentLab.ImageProcessor.Extensions;
  using global::DocumentLab.Interfaces;

  public class DocumentLab : DocumentLabBase, IDocumentLab
  {
    public DocumentLab() : base()
    {
    }

    public InterpreterResult Interpret(string script, Bitmap bitmap)
    {
      var downscaled = imageProcessor.Process(ProcessImageOperation.Downsample, bitmap.ToByteArray(Ocr.Constants.ConvertBetween));

      var ocrResults = ocr.PerformOcr(bitmap, downscaled);
      var page = pageAnalyzer.PerformPageAnalysis(ocrResults);
      var trimmedPage = pageTrimmer.TrimPage(page);

      return interpreter.Interpret(trimmedPage, script);
    }
  }
}
