namespace DocumentLab.Infrastructure
{
  using Castle.Facilities.TypedFactory;
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;
  using Castle.Windsor.Installer;
  using System.Linq;

  public class DocumentLabInstaller : IWindsorInstaller
  {
    private string[] documentLabAssemblies = new string[]
    {
      "DocumentLab.Core",
      "DocumentLab.ImageProcessor",
      "DocumentLab.TextAnalyzer",
      "DocumentLab.PageAnalyzer",
      "DocumentLab.Ocr"
    };

    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.AddFacility<TypedFactoryFacility>();
      documentLabAssemblies.ToList().ForEach(assembly => container.Install(FromAssembly.Named(assembly)));
    }
  }
}
