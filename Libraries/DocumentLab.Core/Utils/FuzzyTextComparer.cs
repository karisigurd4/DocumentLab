namespace DocumentLab.Core.Utils
{
  using System;
    using System.Linq;

    public static class FuzzyTextComparer
  {
    public static bool FuzzyEquals(string script, string label)
    {
      script = script.Replace(" ", "").ToLower();
      label = label.Replace(" ", "").ToLower();

      var minimumLength = new int[] { script.Length, label.Length }.Min();

      var result = label.IndexOf(script, StringComparison.InvariantCultureIgnoreCase) >= 0
        || (minimumLength > 4 && Contains(script, label))
        || (minimumLength > 4 && Contains(label, script))
        || (minimumLength > 4 && LevenshteinDistance.Compute(script, label) <= Constants.LevenshteinDistance);

      return result;
    }

    private static bool Contains(string first, string second)
    {
      for (int i = 0; i < first.Length; i++)
      {
        for (int x = 0; x < second.Length; x++)
        {
          if (first.Length < second.Length)
          {
            return false;
          }

          first = first.Substring(0, second.Length);

          if (LevenshteinDistance.Compute(first, second) <= Constants.LevenshteinDistance)
          {
            return true;
          }
        }
      }

      return false;
    }
  }
}
