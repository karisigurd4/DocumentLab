using System;

namespace DocumentLab.Core.Extensions
{
  public static class ArrayExtensions
  {
    public static int ClosestIndex(this int[] array, int value)
    {
      for (int i = 0; i < array.Length - 1; i++)
      {
        if (value > array[i])
          continue;

        return Math.Abs(value - array[i]) > Math.Abs(value - array[i + 1]) ? i : i + 1;
      }

      return array.Length - 1;
    }

    public static T[,] Slice<T>(this T[,] source, int fromIdxRank0, int toIdxRank0, int fromIdxRank1, int toIdxRank1)
    {
      T[,] ret = new T[toIdxRank0 - fromIdxRank0 + 1, toIdxRank1 - fromIdxRank1 + 1];

      for (int srcIdxRank0 = fromIdxRank0, dstIdxRank0 = 0; srcIdxRank0 <= toIdxRank0; srcIdxRank0++, dstIdxRank0++)
      {
        for (int srcIdxRank1 = fromIdxRank1, dstIdxRank1 = 0; srcIdxRank1 <= toIdxRank1; srcIdxRank1++, dstIdxRank1++)
        {
          dstIdxRank0 = dstIdxRank0 < 0 ? 0 : dstIdxRank0;
          dstIdxRank1 = dstIdxRank1 < 0 ? 0 : dstIdxRank1;
          srcIdxRank0 = srcIdxRank0 < 0 ? 0 : srcIdxRank0;
          srcIdxRank1 = srcIdxRank1 < 0 ? 0 : srcIdxRank1;

          int x0 = dstIdxRank0 > ret.GetLength(0) - 1 ? ret.GetLength(0) - 1 : dstIdxRank0;
          int y0 = dstIdxRank1 > ret.GetLength(1) - 1 ? ret.GetLength(1) - 1 : dstIdxRank1;
          int x1 = srcIdxRank0 > source.GetLength(0) - 1 ? source.GetLength(0) - 1 : srcIdxRank0;
          int y1 = srcIdxRank1 > source.GetLength(1) - 1 ? source.GetLength(1) - 1 : srcIdxRank1;

          ret[x0, y0] = source[x1, y1];
        }
      }
      return ret;
    }
  }
}
