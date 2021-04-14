using System;
using System.Threading;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class LazyAsyncResultWithTimeout : LazyAsyncResult, ICancelableAsyncResult, IAsyncResult
	{
		internal LazyAsyncResultWithTimeout(object workerObject, object callerState, AsyncCallback callback) : base(workerObject, callerState, callback)
		{
		}

		public bool IsCanceled
		{
			get
			{
				return base.InternalPeekCompleted && base.Result is OperationCanceledException;
			}
		}

		public void Cancel()
		{
			base.InvokeCallback(new OperationCanceledException());
		}

		internal void StartTimer(TimeSpan timeout)
		{
			if (this.timer == null)
			{
				this.timer = new Timer(new TimerCallback(this.TimerCallback));
			}
			this.timer.Change(timeout, LazyAsyncResultWithTimeout.Infinite);
		}

		internal void TimeoutOperation(object state)
		{
			base.InvokeCallback(new TimeoutException());
		}

		internal void DisposeTimer()
		{
			if (this.timer != null)
			{
				this.timer.Dispose();
				this.timer = null;
			}
		}

		protected override void Cleanup()
		{
			this.DisposeTimer();
			base.Cleanup();
		}

		private void TimerCallback(object state)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.TimeoutOperation));
		}

		private static readonly TimeSpan Infinite = new TimeSpan(-1L);

		private Timer timer;
	}
}
