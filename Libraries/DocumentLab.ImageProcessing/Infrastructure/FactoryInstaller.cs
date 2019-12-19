namespace DocumentLab.ImageProcessor.Infrastructure
{
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;
  using DocumentLab.ImageProcessor.Interfaces;
  using Castle.Facilities.TypedFactory;
  using DocumentLab.ImageProcessor.Factories;
  using Castle.Core;

  public class FactoryInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(
        Component.For<IProcessImageStrategyFactory>()
          .ImplementedBy<ProcessImageStrategyFactory>()
          .LifeStyle
          .Singleton
#if DEBUG
          .Interceptors(InterceptorReference.ForKey("performanceMetricInterceptor")).Anywhere
#endif     
      );
    }
  }
}
