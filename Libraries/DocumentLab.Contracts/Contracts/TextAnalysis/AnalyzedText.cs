namespace DocumentLab.Contracts
{
  using System;
  using System.Drawing;

  public class AnalyzedText : IEquatable<AnalyzedText>
  {
    public string TextType { get; set; }
    public Rectangle BoundingBox { get; set; }
    public string Text { get; set; }

    public bool Equals(AnalyzedText other)
    {
      return (this.GetHashCode() == other.GetHashCode());
    }

    public override int GetHashCode()
    {
      return string
        .Format("{0}{1}", TextType, Text)
        .GetHashCode();
    }
  }
}
