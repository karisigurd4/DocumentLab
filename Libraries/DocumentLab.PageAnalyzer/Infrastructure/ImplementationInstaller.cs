namespace DocumentLab.PageAnalyzer.Infrastructure
{
  using Interfaces;
  using Implementation;
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;

  public class ImplementationInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(
        Component.For<IPageAnalyzer>()
          .ImplementedBy<PageAnalyzer>()
          .LifeStyle
          .Singleton,
        Component.For<IPageTrimmer>()
          .ImplementedBy<PageTrimmer>()
          .LifeStyle
          .Singleton
      );
    }
  }
}
