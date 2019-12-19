namespace DocumentLab.Core.Extensions
{
  using System;

  public static class IntExtensions
  {
    public static int RoundOff(this int i, int factor = 10)
    {
      return (int)((Math.Round(i / (double)factor)) * factor);
    }
  }
}
