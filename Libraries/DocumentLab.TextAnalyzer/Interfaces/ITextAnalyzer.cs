namespace DocumentLab.TextAnalyzer.Interfaces
{
  using DocumentLab.Contracts;
  using System.Collections.Generic;

  public interface ITextAnalyzer
  {
    IEnumerable<AnalyzedText> AnalyzeOcrResult(OcrResult ocrResult);
    IEnumerable<AnalyzedText> AnalyzeString(string value);
  }
}
