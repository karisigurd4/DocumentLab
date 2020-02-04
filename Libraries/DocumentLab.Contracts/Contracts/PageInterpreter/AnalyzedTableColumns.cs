namespace DocumentLab.Contracts.PageInterpreter
{
  using System;

  public class AnalyzedTableColumns
  {
    public PageUnit[] Rows { get; set; }

    public int ColumnIndex { get; set; }
  }
}
