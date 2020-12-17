namespace DocumentLab.Web
{
	using Infrastructure;
	using Newtonsoft.Json;
	using System.Web.Http;
	using System.Web.Http.Dispatcher;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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

			// Manually resolve a service descriptor to read the ApiVersion for use in the routing before releasing it again.
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
