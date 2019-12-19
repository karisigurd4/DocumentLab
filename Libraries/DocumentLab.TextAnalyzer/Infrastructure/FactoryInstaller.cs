namespace DocumentLab.TextAnalyzer.Infrastructure
{
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;
  using DocumentLab.TextAnalyzer.Interfaces;
  using Castle.Facilities.TypedFactory;
  using DocumentLab.TextAnalyzer.Factories;
  using Castle.Core;

  public class FactoryInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(
        Component.For<IAnalyzeTextStrategyFactory>()
          .ImplementedBy<AnalyzeTextStrategyFactory>()
          .LifeStyle
          .Singleton
#if DEBUG
          .Interceptors(InterceptorReference.ForKey("performanceMetricInterceptor")).Anywhere
#endif  
      );
    }
  }
}
