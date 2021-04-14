using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MapiHttp;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RpcCallContext<TResult> : ClientCancelableCallContext<TResult> where TResult : RpcCallResult
	{
		public RpcCallContext(TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
		{
			this.timeout = timeout;
			this.timer = null;
			this.completedTimer = 0;
		}

		protected abstract TResult OnRpcException(RpcException rpcException);

		protected virtual TResult OnProtocolException(ProtocolException protocolException)
		{
			throw new NotImplementedException("OnProtocolException");
		}

		protected override TResult ConvertExceptionToResult(Exception exception)
		{
			RpcException ex = exception as RpcException;
			if (ex != null)
			{
				return this.OnRpcException(ex);
			}
			ProtocolException ex2 = exception as ProtocolException;
			if (ex2 != null)
			{
				return this.OnProtocolException(ex2);
			}
			AggregateException ex3 = exception as AggregateException;
			if (ex3 != null)
			{
				foreach (Exception ex4 in ex3.Flatten().InnerExceptions)
				{
					ex2 = (ex4 as ProtocolException);
					if (ex2 != null)
					{
						return this.OnProtocolException(ex2);
					}
				}
			}
			return default(TResult);
		}

		protected void StartRpcTimer()
		{
			this.timer = new Timer(new TimerCallback(this.TimeoutCallback), null, this.timeout, TimeSpan.FromMilliseconds(-1.0));
		}

		protected void StopAndCleanupRpcTimer()
		{
			if (Interlocked.CompareExchange(ref this.completedTimer, 1, 0) == 0)
			{
				this.TimerCleanup();
			}
		}

		private void TimeoutCallback(object state)
		{
			if (Interlocked.CompareExchange(ref this.completedTimer, 1, 0) == 0)
			{
				base.DeferExceptions<object>(delegate(object unused)
				{
					this.TimerCleanup();
					base.Cancel();
				}, null);
			}
		}

		private void TimerCleanup()
		{
			if (this.timer != null)
			{
				this.timer.Change(-1, -1);
				this.timer.Dispose();
				this.timer = null;
			}
		}

		private readonly TimeSpan timeout;

		private Timer timer;

		private int completedTimer;
	}
}
