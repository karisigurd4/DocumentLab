namespace DocumentLab.ImageProcessor.Infrastructure
{
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;
  using DocumentLab.ImageProcessor.Interfaces;
  using DocumentLab.ImageProcessor.Implementation;

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
