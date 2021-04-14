using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ExecuteAsyncResult : EasyAsyncResult, IDisposeTrackable, IDisposable
	{
		public ExecuteAsyncResult(Func<ExecuteAsyncResult, ExecuteContext> executeContextFactory, AsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
		{
			this.executeContext = executeContextFactory(this);
			this.disposeTracker = DisposeTracker.Get<ExecuteAsyncResult>(this);
		}

		public ExecuteContext ExecuteContext
		{
			get
			{
				return this.executeContext;
			}
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				Util.DisposeIfPresent(this.executeContext);
				Util.DisposeIfPresent(this.disposeTracker);
				GC.SuppressFinalize(this);
				this.isDisposed = true;
			}
		}

		DisposeTracker IDisposeTrackable.GetDisposeTracker()
		{
			return DisposeTracker.Get<ExecuteAsyncResult>(this);
		}

		void IDisposeTrackable.SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private readonly ExecuteContext executeContext;

		private readonly DisposeTracker disposeTracker;

		private bool isDisposed;
	}
}
