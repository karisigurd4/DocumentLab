namespace DocumentLab.Core.Extensions
{
  using System;
  using System.Collections.Generic;

  public static class ListExtensions
  {
    public static List<List<T>> SplitList<T>(this List<T> original, int size = 50)
    {
      var list = new List<List<T>>();

      for (int i = 0; i < original.Count; i += size)
        list.Add(original.GetRange(i, Math.Min(size, original.Count - i)));

      return list;
    }
  }
}
