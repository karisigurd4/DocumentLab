namespace DocumentLab.Web.Infrastructure
{
	using System;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}