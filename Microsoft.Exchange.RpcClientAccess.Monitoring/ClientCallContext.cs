using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ClientCallContext<TResult> : EasyAsyncResult where TResult : CallResult
	{
		public ClientCallContext(AsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
		{
		}

		public IAsyncResult Begin()
		{
			try
			{
				this.latencyTracker.Start();
				this.OnBegin(new AsyncCallback(ClientCallContext<TResult>.InternalCallback), this);
			}
			catch (Exception ex)
			{
				this.result = this.ConvertExceptionToResult(ex);
				if (this.result == null)
				{
					this.exception = ex;
				}
				base.InvokeCallback();
			}
			return this;
		}

		protected abstract IAsyncResult OnBegin(AsyncCallback asyncCallback, object asyncState);

		protected abstract TResult OnEnd(IAsyncResult asyncResult);

		protected TResult GetResult()
		{
			base.WaitForCompletion();
			if (this.exception != null)
			{
				throw this.exception;
			}
			TResult tresult = this.PostProcessResult(this.result);
			tresult.Validate();
			return tresult;
		}

		protected virtual TResult ConvertExceptionToResult(Exception exception)
		{
			return default(TResult);
		}

		protected virtual TResult PostProcessResult(TResult result)
		{
			result.Latency = this.latencyTracker.Elapsed;
			result.Validate();
			return result;
		}

		private static void InternalCallback(IAsyncResult asyncResult)
		{
			ClientCallContext<TResult> clientCallContext = (ClientCallContext<TResult>)asyncResult.AsyncState;
			clientCallContext.InternalEnd(asyncResult);
		}

		private void InternalEnd(IAsyncResult asyncResult)
		{
			this.latencyTracker.Stop();
			try
			{
				this.result = this.OnEnd(asyncResult);
			}
			catch (Exception ex)
			{
				this.result = this.ConvertExceptionToResult(ex);
				if (this.result == null)
				{
					this.exception = ex;
				}
			}
			base.InvokeCallback();
		}

		private TResult result;

		private Exception exception;

		private Stopwatch latencyTracker = new Stopwatch();
	}
}
