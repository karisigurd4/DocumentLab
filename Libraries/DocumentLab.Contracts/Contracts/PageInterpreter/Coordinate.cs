namespace DocumentLab.Contracts.Contracts.PageInterpreter
{
  using System;

  public class Coordinate : ICloneable, IEquatable<Coordinate>
  {
    public int X { get; set; }
    public int Y { get; set; }

    public Coordinate()
    {
    }

    public Coordinate(int x, int y)
    {
      this.X = x;
      this.Y = y;
    }

    public object Clone()
    {
      return new Coordinate
      (
        X = X,
        Y = Y
      );
    }

    public bool Equals(Coordinate other)
    {
      return this.X == other.X && this.Y == other.Y;
    }
  }
}
