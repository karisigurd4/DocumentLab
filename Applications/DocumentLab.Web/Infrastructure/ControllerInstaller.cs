namespace DocumentLab.Web.Infrastructure
{
	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.SubSystems.Configuration;
	using Castle.Windsor;
	using System.Web.Http.Controllers;

	/// <summary>
	/// 
	/// </summary>
	public class ControllerInstaller : IWindsorInstaller
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="container"></param>
		/// <param name="store"></param>
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(
				Classes
					.FromAssembly(typeof(ContainerApplication).Assembly)
					.BasedOn<IHttpController>()
					.LifestyleTransient());
		}
	}
}