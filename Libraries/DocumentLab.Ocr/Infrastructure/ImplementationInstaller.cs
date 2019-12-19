namespace DocumentLab.Ocr.Infrastructure
{
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;
  using DocumentLab.Ocr.Interfaces;
  using DocumentLab.Ocr.Implementation;
  using Castle.Core;
  using Castle.Windsor.Installer;

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
