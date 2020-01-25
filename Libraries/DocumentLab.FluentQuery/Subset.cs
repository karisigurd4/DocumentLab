namespace DocumentLab
{
  using DocumentLab.Contracts.Enums.Types;

  public class Subset
  {
    internal int Percentage { get; set; }
    internal SubsetType SubsetType { get; set; }
    
    internal Subset()
    {

    }

    public static Subset Top(int percentage)
    {
      return new Subset()
      {
        Percentage = percentage,
        SubsetType = SubsetType.Top
      };
    }

    public static Subset Bottom(int percentage)
    {
      return new Subset()
      {
        Percentage = percentage,
        SubsetType = SubsetType.Bottom
      };
    }

    public static Subset Left(int percentage)
    {
      return new Subset()
      {
        Percentage = percentage,
        SubsetType = SubsetType.Left
      };
    }

    public static Subset Right(int percentage)
    {
      return new Subset()
      {
        Percentage = percentage,
        SubsetType = SubsetType.Right
      };
    }

    public override string ToString()
    {
      return $"{SubsetType.ToString()} {Percentage}";
    }
  }
}
