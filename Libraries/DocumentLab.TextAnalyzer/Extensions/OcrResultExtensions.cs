namespace DocumentLab.TextAnalyzer.Extensions
{
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.Enums.Types;
  using DocumentLab.Contracts.Extensions;
  using DocumentLab.Core.Extensions;
  using DocumentLab.Core.Storage;
  using DocumentLab.Core.Utils;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public static class OcrResultExtensions
  {
    public static IEnumerable<AnalyzedText> BinarySearchMatchFromFile(this OcrResult ocrResult, string textType, string filePath, bool useFuzzyMatch = true)
    {
      var allWords = ocrResult.AllWords().ToArray();
      Constants.SpecialCharactersTranslationDictionary.ForEach(x =>
      {
        allWords = allWords.Replace(x.Key, x.Value);
      });

      List<string> resultList = new List<string>();
      return allWords
        .Select(x => FuzzyBinarySearch.FindCloseMatch(x, filePath, useFuzzyMatch))
        .Where(x => x != null)
        .Distinct()
        .Select(x => new AnalyzedText()
        {
          Text = x,
          TextType = textType,
          BoundingBox = ocrResult.BoundingBox
        });
    }
  }
}
