namespace DocumentLab.Contracts.Contracts.PageInterpreter
{
  using System;

  public class AnalyzedTableColumns
  {
    public PageUnit[] Rows { get; set; }

    public int RuleIndex { get; set; }
  }
}
