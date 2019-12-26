namespace DocumentLab
{
  using System.Drawing;
  using Contracts.Enums.Operations;
  using ImageProcessor.Extensions;
  using Interfaces;
  using PageInterpreter;

  public class DocumentLab : DocumentLabBase, IDocumentLab
  {
    public DocumentLab() : base() { }

    public string InterpretToJson(string script, Bitmap bitmap)
    {
      var downscaled = imageProcessor.Process(ProcessImageOperation.Downsample, bitmap.ToByteArray(Ocr.Constants.ConvertBetween));

      var ocrResults = ocr.PerformOcr(bitmap, downscaled);
      var page = pageAnalyzer.PerformPageAnalysis(ocrResults);
      var trimmedPage = pageTrimmer.TrimPage(page);

      return interpreter.Interpret(trimmedPage, script).ConvertToJson(script);
    }
  }
}
