namespace DocumentLab.Core.Builders
{
  using AutoCacheLib.Implementation;
  using DocumentLab.Core.Interfaces;
  using System;
  using System.Linq;
  using System.Runtime.Caching;

  public class StrategyFactoryBuilder : IStrategyFactoryBuilder
  {
    public T BuildStrategyFactory<T>(Enum operation)
    {
      return AutoCache.CacheRetrieve(() =>
      {
        var strategyType = AppDomain
          .CurrentDomain
          .GetAssemblies()
          .Where(x => x.FullName.StartsWith(Constants.ApplicationGlobalNamespace))
          .SelectMany(x => x.GetTypes())
          .FirstOrDefault(t => t.Name == operation.ToString() + Constants.StrategySuffix);

        if (strategyType == null)
        {
#if DEBUG
          throw new NotImplementedException(Constants.MissingStrategyImplementation(operation.ToString() + Constants.StrategySuffix));
#else
          return default(T);
#endif
        }

        return (T)Activator.CreateInstance(strategyType);
      }, new CacheItemPolicy()
      {
        Priority = CacheItemPriority.NotRemovable
      });
    }
  }
}
