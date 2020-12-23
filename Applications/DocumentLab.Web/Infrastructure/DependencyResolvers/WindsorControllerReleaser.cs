namespace DocumentLab.Web.Infrastructure
{
	using System;

	public class WindsorControllerReleaser : IDisposable
	{
		private readonly Action release;

		public WindsorControllerReleaser(Action release)
		{
			this.release = release;
		}

		public void Dispose()
		{
			this.release();
		}
	}
}