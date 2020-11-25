namespace DocumentLab.Contracts.PageInterpreter
{
  using System.Collections.Generic;
  using System.Linq;

  public class PatternResult
  {
    public Dictionary<string, string> Result { get; set; } = new Dictionary<string, string>();

    private int anyCounter = 0;

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
        Result.Add(key + (anyCounter++).ToString(), value);
      }
      else
      {
        Result.Add(key ?? Result.Count.ToString(), value);
      }
    }
  }
}
