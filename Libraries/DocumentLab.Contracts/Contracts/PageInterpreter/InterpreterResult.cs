namespace DocumentLab.Contracts.PageInterpreter
{
  using System.Collections.Generic;

  public class InterpreterResult
  {
    public Dictionary<string, PatternResult> Results { get; } = new Dictionary<string, PatternResult>();

    public void AddResult(string label, string customProperty, string value)
    {
      if (string.IsNullOrWhiteSpace(label))
      {
        label = Results.Count.ToString();
      }

      if (!Results.ContainsKey(label))
      {
        Results.Add(label, new PatternResult());
      }

      Results[label].AddResult(customProperty, value);
    }
  }
}
