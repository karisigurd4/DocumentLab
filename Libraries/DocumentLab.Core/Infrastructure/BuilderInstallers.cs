namespace DocumentLab.Core.Infrastructure
{
  using Castle.Core;
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;
  using DocumentLab.Core.Builders;
  using DocumentLab.Core.Interfaces;

  public class BuilderInstallers : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(
        Component.For<IStrategyFactoryBuilder>()
          .ImplementedBy<StrategyFactoryBuilder>()
          .LifeStyle
          .Singleton
      );
    }
  }
}
