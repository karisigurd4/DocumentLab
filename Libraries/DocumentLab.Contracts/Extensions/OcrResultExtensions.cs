namespace DocumentLab.Contracts.Extensions
{
  using DocumentLab.Contracts;
  using DocumentLab.Core.Extensions;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;

  public static class OcrResultExtensions
  {
    public static string AsContinuousText(this OcrResult ocrResult, bool spaceBetweenLines = true)
    {
      var result = new string(
        ocrResult
          .Result
          .SelectMany(x => x + '»')
          .Where(c => !char.IsWhiteSpace(c))
          .ToArray()
      );

      if (spaceBetweenLines)
        return result.Replace("»", " ");

      return result.Replace("»", string.Empty);
    }

    public static string AsString(this OcrResult ocrResult)
    {
      return new string(
          ocrResult
            .Result
            .SelectMany(x => x + " ")
            .ToArray()
        );
    }

    public static IEnumerable<string> AllWords(this OcrResult ocrResult)
    {
      return Regex.Split(ocrResult.AsString().Replace(',', ' '), @"\s+");
    }

    public static IEnumerable<OcrResult> ReplaceFaultyCharacters(this IEnumerable<OcrResult> ocrResults, Dictionary<string, string> ocrFix)
    {
      ocrResults.ForEach(x =>
      {
        ocrFix.ForEach(y =>
        {
          x.Result.Replace(y.Key, y.Value);
        });
      });

      return ocrResults;
    }

    public static IEnumerable<string> ReplaceFaultyCharacters(this IEnumerable<string> strings, Dictionary<string, string> ocrFix)
    {
      var replacedStrings = strings.ToArray();

      for (int i = 0; i < replacedStrings.Length; i++)
      {
        ocrFix.ForEach(y =>
        {
          replacedStrings = replacedStrings.Replace(y.Key, y.Value);
        });
      }

      return strings;
    }

    public static string ReplaceFaultyCharacters(this string input, Dictionary<string, string> ocrFix)
    {
      ocrFix.ForEach(y =>
      {
        input = input.Replace(y.Key, y.Value);
      });

      return input;
    }
  }
}
