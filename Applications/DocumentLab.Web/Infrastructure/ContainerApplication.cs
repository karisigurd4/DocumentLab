namespace DocumentLab.Web.Infrastructure
{
	using Castle.Windsor;
	using Castle.Windsor.Installer;
	using System;
	using System.IO;

	public abstract class ContainerApplication : System.Web.HttpApplication
	{
		private static IWindsorContainer container;

		protected IWindsorContainer Container
		{
			get { return container; }
			set { container = value; }
		}

		protected ContainerApplication()
		{
		}

		protected void Application_Start(object sender, EventArgs e)
		{
			Initialise();
		}

		protected virtual void Initialise()
		{
			Container = CreateContainer();
			RunInstallers();
			AppStart();
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		protected abstract void AppStart();

		protected virtual void RunInstallers()
		{
			Container.Install(FromAssembly.This());
		}
		protected virtual IWindsorContainer CreateContainer() => new WindsorContainer();
	}
}