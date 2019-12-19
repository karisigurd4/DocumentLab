namespace DocumentLab.TextAnalyzer.Interfaces
{
  using DocumentLab.Contracts;
  using System.Collections.Generic;

  public interface IAnalyzeTextStrategy
  {
    IEnumerable<AnalyzedText> Analyze(OcrResult ocrResult);
  }
}
