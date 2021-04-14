using System;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal abstract class MsiBase : IDisposable
	{
		protected MsiBase()
		{
			this.disposed = false;
		}

		public SafeMsiHandle Handle { get; set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this.Handle != null && !this.Handle.IsInvalid)
				{
					this.Handle.Dispose();
				}
				this.Handle = null;
				this.disposed = true;
			}
		}

		private bool disposed;
	}
}
