namespace DocumentLab.TextAnalyzer.Interfaces
{
  using Contracts;
  using Contracts.Ocr;
  using System.Collections.Generic;

  public interface IAnalyzeTextStrategy
  {
    IEnumerable<AnalyzedText> Analyze(OcrResult ocrResult);
  }
}
