namespace DocumentLab.ImageProcessor.Infrastructure
{
  using Interfaces;
  using Implementation;
  using Castle.Windsor;
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;

  public class ImplementationInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(
        Component.For<IImageAnalyzer>()
          .ImplementedBy<ImageAnalyzer>()
          .LifeStyle
          .Singleton
        ,Component.For<IImageProcessor>()
          .ImplementedBy<ImageProcessor>()
          .LifeStyle
          .Singleton
      );
    }
  }
}
