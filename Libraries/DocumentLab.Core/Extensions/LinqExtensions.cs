namespace DocumentLab.Core.Extensions
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public static class LinqExtensions
  {
    public static IEnumerable<string> Replace(this IEnumerable<string> enumerable, string oldValue, string newValue)
    {
      return enumerable.Select(x => x.Replace(oldValue, newValue));
    }

    public static string[] Replace(this string[] array, string oldValue, string newValue)
    {
      return array.Select(x => x.Replace(oldValue, newValue)).ToArray();
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
      foreach (T item in enumerable)
      {
        action(item);
      }
    }
  }
}
