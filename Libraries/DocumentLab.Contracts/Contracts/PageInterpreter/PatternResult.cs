namespace DocumentLab.Contracts.PageInterpreter
{
  using System.Collections.Generic;
  using System.Linq;

  public class PatternResult
  {
    public Dictionary<string, string> Result { get; set; } = new Dictionary<string, string>();

    public string GetResultAt(int index)
    {
      return Result.ElementAt(index).Value;
    }

    public string GetResultByKey(string key)
    {
      if (!Result.ContainsKey(key))
        return null;

      return Result[key];
    }

    public void AddResult(string key, string value)
    {
      if (Result.ContainsKey(key ?? Result.Count.ToString()))
      {
        Result[key] = value;
      }

      Result.Add(key ?? Result.Count.ToString(), value);
    }
  }
}
