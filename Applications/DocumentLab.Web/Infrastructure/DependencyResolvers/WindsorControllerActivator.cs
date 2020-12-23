namespace DocumentLab.Web.Infrastructure
{
	using Castle.Windsor;
	using System;
	using System.Net.Http;
	using System.Web.Http.Controllers;
	using System.Web.Http.Dispatcher;

	public class WindsorControllerActivator : IHttpControllerActivator
	{

		private IWindsorContainer container;
		public WindsorControllerActivator(IWindsorContainer container)
		{
			this.container = container;
		}
		public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
		{
			var controller = container.Resolve(controllerType);
			request.RegisterForDispose(new WindsorControllerReleaser(() => this.container.Release(controller)));
			return controller as IHttpController;
		}
	}
}