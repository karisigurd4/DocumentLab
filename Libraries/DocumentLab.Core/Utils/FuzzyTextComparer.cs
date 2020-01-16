namespace DocumentLab.Core.Utils
{
  public static class FuzzyTextComparer
  {
    public static bool FuzzyEquals(string realText, string labelText)
    {
      var possibleParts = labelText.Split(' ');
      var realParts = realText.Split(' ');

      foreach (var possiblePart in possibleParts)
      {
        foreach (var realPart in realParts)
        {
          if (FuzzyEqualsWithAny(realPart, possiblePart))
            return false;
        }
      }

      return true;
    }

    private static bool FuzzyEqualsWithAny(string realText, string possiblyFuzzyText)
    {
      realText = realText.Replace(" ", string.Empty);
      possiblyFuzzyText = possiblyFuzzyText.Replace(" ", string.Empty);

      for (int i = 0; i < possiblyFuzzyText.Length; i++)
      {
        string shortened = SafeSubstring(possiblyFuzzyText, i, realText.Length);

        if (LevenshteinDistance.Compute(realText, shortened) < 1)
          return true;
      }

      return false;
    }

    private static string SafeSubstring(string str, int start, int length)
    {
      if (str.Length < start)
      {
        return string.Empty;
      }

      if (str.Length < start + length)
      {
        length = str.Length - start;
      }

      return str.Substring(start, length);
    }
  }
}
