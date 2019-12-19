namespace DocumentLab.Core.Utils
{
  using DocumentLab.Core.Storage;
  using System.Linq;

  public static class FuzzyBinarySearch
  {
    public static string FindCloseMatch(string text, string fileName, bool useFuzzyMatch)
    {
      var orderedList = FileReader
        .GetFileLinesOrdered(fileName)
        .ToList();

      int min = 0;
      int max = orderedList.Count - 1;

      while (min <= max)
      {
        int mid = (min + max) / 2;


        if (useFuzzyMatch && text.Length > Constants.FuzzyStringLengthLimit && LevenshteinDistance.Compute(orderedList[mid], text) < 2)
        {
          return orderedList[mid];
        }
        else if (orderedList[mid].Equals(text, System.StringComparison.InvariantCultureIgnoreCase))
        {
          return orderedList[mid];
        }
        else if (text.CompareTo(orderedList[mid]) < 0)
        {
          max = mid - 1;
        }
        else
        {
          min = mid + 1;
        }
      }

      return null;
    }
  }
}
