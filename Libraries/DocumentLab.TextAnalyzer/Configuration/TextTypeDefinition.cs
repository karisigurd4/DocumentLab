namespace DocumentLab.TextAnalyzer.Configuration
{
  using System;
  
  public enum GetTextAs
  {
    Text,
    Continuous
  }

  public class ReplaceDefintion
  {
    public string[] Find { get; set; }
    public string Replace { get; set; }
  }

  public class TextDefinition
  {
    public GetTextAs GetAs { get; set; }
    public ReplaceDefintion[] Replace { get; set; }
  }

  public class TextTypeDefinition
  {
    public string Name { get; set; }
    public TextDefinition Text { get; set; }
    public string[] Regexes { get; set; }
  }
}
