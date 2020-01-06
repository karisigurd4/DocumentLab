namespace DocumentLab.TextAnalyzer.Interfaces
{
  using Contracts;
  using Contracts.Ocr;
  using System.Collections.Generic;

  public interface ITextAnalyzer
  {
    IEnumerable<AnalyzedText> AnalyzeOcrResult(OcrResult ocrResult);
    IEnumerable<AnalyzedText> AnalyzeString(string value);
  }
}
