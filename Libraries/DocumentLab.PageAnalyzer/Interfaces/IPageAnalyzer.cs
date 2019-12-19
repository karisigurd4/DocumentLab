namespace DocumentLab.PageAnalyzer.Interfaces
{
  using DocumentLab.Contracts;
  using System.Collections.Generic;

  public interface IPageAnalyzer
  {
    Page PerformPageAnalysis(IEnumerable<OcrResult> analyzedTexts);
    int[] GetHorizontalIndices(IEnumerable<OcrResult> analyzedTexts);
    int[] GetVerticalIndices(IEnumerable<OcrResult> analyzedTexts);
  }
}
