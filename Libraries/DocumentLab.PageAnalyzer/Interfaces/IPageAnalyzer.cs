namespace DocumentLab.PageAnalyzer.Interfaces
{
  using Contracts;
  using Contracts.Ocr;
  using System.Collections.Generic;

  public interface IPageAnalyzer
  {
    Page PerformPageAnalysis(IEnumerable<OcrResult> analyzedTexts);
    int[] GetHorizontalIndices(IEnumerable<OcrResult> analyzedTexts);
    int[] GetVerticalIndices(IEnumerable<OcrResult> analyzedTexts);
  }
}
