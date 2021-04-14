using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal class ExecutorContext : DisposeTrackableBase
	{
		public ExecutorContext()
		{
			this.Failures = new ConcurrentBag<Exception>();
			this.CancellationTokenSource = new CancellationTokenSource();
			this.WaitHandle = new ManualResetEvent(false);
		}

		public CancellationTokenSource CancellationTokenSource { get; private set; }

		public ManualResetEvent WaitHandle { get; private set; }

		public Exception FatalException { get; set; }

		public ConcurrentBag<Exception> Failures { get; private set; }

		public object Input { get; set; }

		public object Output { get; set; }

		protected override void InternalDispose(bool disposing)
		{
			this.WaitHandle.Dispose();
			this.CancellationTokenSource.Dispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExecutorContext>(this);
		}
	}
}
