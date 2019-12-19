namespace DocumentLab.Core.Infrastructure
{
  using Castle.DynamicProxy;
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;
  using DocumentLab.Core.Interceptors;

  public class InterceptorInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(
        Component.For<IInterceptor>()
          .ImplementedBy<PerformanceMetricInterceptor>()
          .Named("performanceMetricInterceptor")
      );
    }
  }
}
