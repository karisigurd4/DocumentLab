namespace DocumentLab.Core.Interfaces
{
  using System;

  public interface IStrategyFactoryBuilder
  {
    T BuildStrategyFactory<T>(Enum operation);
  }
}
