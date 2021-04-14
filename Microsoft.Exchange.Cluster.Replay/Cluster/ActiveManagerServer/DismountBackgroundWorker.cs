using System;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DismountBackgroundWorker : IDisposable
	{
		private IAsyncResult AsyncResult { get; set; }

		private DismountBackgroundWorker.DismountDelegate Func { get; set; }

		public ManualResetEvent CompletedEvent { get; private set; }

		public Exception DismountException { get; private set; }

		public ExDateTime StartTime { get; private set; }

		public FileIOonSourceException LastE00ReadException { get; set; }

		public DismountBackgroundWorker(DismountBackgroundWorker.DismountDelegate func)
		{
			this.Func = func;
			this.CompletedEvent = new ManualResetEvent(false);
		}

		public void Start()
		{
			this.StartTime = ExDateTime.Now;
			this.AsyncResult = this.Func.BeginInvoke(delegate(IAsyncResult ar)
			{
				this.DismountException = this.Func.EndInvoke(ar);
				this.CompletedEvent.Set();
			}, null);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			this.Dispose(true);
		}

		public void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.CompletedEvent.Close();
				this.CompletedEvent = null;
			}
		}

		public delegate Exception DismountDelegate();
	}
}
