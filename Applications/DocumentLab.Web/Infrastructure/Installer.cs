namespace DocumentLab.Web.Infrastructure
{
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;
  using Castle.Windsor.Installer;

  public class Installer : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
    }
  }
}