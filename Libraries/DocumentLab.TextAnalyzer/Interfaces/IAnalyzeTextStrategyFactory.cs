namespace DocumentLab.TextAnalyzer.Interfaces
{
  using DocumentLab.Contracts.Enums;

  public interface IAnalyzeTextStrategyFactory
  {
    IAnalyzeTextStrategy CreateStrategy(AnalyzeTextOperation operation);
  }
}
