namespace DocumentLab.Ocr.Infrastructure
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
        Component.For<IOcr>()
          .ImplementedBy<Ocr>()
          .LifeStyle
          .Singleton,

        Component.For<ITesseractPool>()
          .ImplementedBy<TesseractPool>()
          .LifeStyle
          .Singleton
      );
    }
  }
}
