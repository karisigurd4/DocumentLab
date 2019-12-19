namespace DocumentLab.Core.Extensions
{
  using System;

  public static class StringExtensions
  {
    public static T ToEnum<T>(this string enumString)
    {
      return (T)Enum.Parse(typeof(T), enumString);
    }
  }
}
