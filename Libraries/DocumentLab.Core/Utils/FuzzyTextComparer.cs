namespace DocumentLab.Core.Utils
{
  using System;

  public static class FuzzyTextComparer
  {
    public static bool FuzzyEquals(string script, string label)
    {
      script = script.Replace(" ", "").ToLower();
      label = label.Replace(" ", "").ToLower();

      return label.IndexOf(script, StringComparison.InvariantCultureIgnoreCase) >= 0
        || Contains(script, label)
        || LevenshteinDistance.Compute(script, label) <= Constants.LevenshteinDistance;
    }

    private static bool Contains(string first, string second)
    {
      var shorter = first.Length > second.Length ? second : first;
      var longer = first.Length < second.Length ? second : first;

      for (int i = 0; i < longer.Length; i++)
      {
        for (int x = 0; x < shorter.Length; x++)
        {
          if (longer.Length < shorter.Length)
          {
            return false;
          }

          var longerSub = longer.Substring(0, shorter.Length);

          if (LevenshteinDistance.Compute(shorter, longerSub) <= Constants.LevenshteinDistance)
          {
            return true;
          }
        }
      }

      return false;
    }
  }
}
