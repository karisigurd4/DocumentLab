namespace DocumentLab.ImageProcessor.Interfaces
{
  using DocumentLab.Contracts.Enums;

  public interface IProcessImageStrategyFactory
  {
    IProcessImageStrategy CreateStrategy(ProcessImageOperation operation);
  }
}
