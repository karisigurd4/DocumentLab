namespace DocumentLab.Web.Infrastructure
{
	using Castle.Windsor;
	using Castle.Windsor.Installer;
	using System;
	using System.IO;

	/// <summary>
	/// 
	/// </summary>
	public abstract class ContainerApplication : System.Web.HttpApplication
	{
		private static IWindsorContainer container;

		/// <summary>
		/// 
		/// </summary>
		protected IWindsorContainer Container
		{
			get { return container; }
			set { container = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		protected ContainerApplication()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_Start(object sender, EventArgs e)
		{
			Initialise();
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Initialise()
		{
			// Make it possible to load unmanaged libsodium DLLs that .NET does not make shadow copies of.
			// -> Simply point the "path" variable to the Bin directory.
			string path = Environment.GetEnvironmentVariable("PATH");
			string binDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin");
			Environment.SetEnvironmentVariable("PATH", path + ";" + binDir);

			Container = CreateContainer();
			RunInstallers();
			AppStart();
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();
		}

		/// <summary>
		/// 
		/// </summary>
		protected abstract void AppStart();

		/// <summary>
		/// 
		/// </summary>
		protected virtual void RunInstallers()
		{
			Container.Install(FromAssembly.This());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual IWindsorContainer CreateContainer() => new WindsorContainer();
	}
}