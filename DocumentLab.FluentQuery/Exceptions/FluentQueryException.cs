namespace DocumentLab
{
  using System;
  
  public class FluentQueryException : Exception
  {
    public FluentQueryException(string message)
      : base(message)
    {
    }
  }
}
