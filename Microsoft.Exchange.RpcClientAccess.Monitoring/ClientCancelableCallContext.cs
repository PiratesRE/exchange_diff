using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ClientCancelableCallContext<TResult> : EasyAsyncResult where TResult : CallResult
	{
		public ClientCancelableCallContext(AsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
		{
		}

		public IAsyncResult Begin()
		{
			if (!this.DeferExceptions<object>(delegate(object unused)
			{
				this.latencyTracker.Start();
				this.cancelableAsyncResult = this.OnBegin(delegate(ICancelableAsyncResult asyncResult)
				{
					((ClientCancelableCallContext<TResult>)asyncResult.AsyncState).InternalEnd(asyncResult);
				}, this);
			}, null))
			{
				this.InternalEnd(null);
			}
			return this;
		}

		protected abstract ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState);

		protected abstract TResult OnEnd(ICancelableAsyncResult asyncResult);

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

		protected void Cancel()
		{
			if (this.cancelableAsyncResult != null)
			{
				this.cancelableAsyncResult.Cancel();
			}
		}

		protected virtual TResult PostProcessResult(TResult result)
		{
			result.Latency = this.latencyTracker.Elapsed;
			result.Validate();
			return result;
		}

		protected bool DeferExceptions<TArgIn>(Action<TArgIn> guardedAction, TArgIn arg)
		{
			bool flag;
			try
			{
				guardedAction(arg);
				flag = true;
			}
			catch (Exception ex)
			{
				this.result = this.ConvertExceptionToResult(ex);
				if (this.result == null)
				{
					this.exception = ex;
				}
				flag = false;
			}
			return flag;
		}

		private void InternalEnd(ICancelableAsyncResult asyncResult)
		{
			this.latencyTracker.Stop();
			if (asyncResult != null)
			{
				this.cancelableAsyncResult = asyncResult;
				this.DeferExceptions<ICancelableAsyncResult>(delegate(ICancelableAsyncResult r)
				{
					this.result = this.OnEnd(r);
				}, asyncResult);
			}
			base.InvokeCallback();
		}

		private TResult result;

		private Exception exception;

		private ICancelableAsyncResult cancelableAsyncResult;

		private Stopwatch latencyTracker = new Stopwatch();
	}
}
