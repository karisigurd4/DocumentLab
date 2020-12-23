namespace DocumentLab.Web
{
	using Infrastructure;
	using Newtonsoft.Json;
	using System.Web.Http;
	using System.Web.Http.Dispatcher;

	public class WebApiApplication : ContainerApplication
	{
		protected override void AppStart()
		{
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings
			{
				Formatting = Newtonsoft.Json.Formatting.Indented,
				ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			};
			ConfigureDependencyResolvers();
			GlobalConfiguration.Configure(config => WebApiConfig.Register(config));
			GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
		}

		private void ConfigureDependencyResolvers()
		{
			GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorControllerActivator(this.Container));
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
