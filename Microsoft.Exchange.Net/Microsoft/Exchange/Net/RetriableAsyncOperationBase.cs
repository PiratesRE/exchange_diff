using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal abstract class RetriableAsyncOperationBase<TClient, TAsyncResult> where TAsyncResult : BasicAsyncResult
	{
		protected RetriableAsyncOperationBase(IServiceProxyPool<TClient> proxyPool, TAsyncResult callerAsyncResult, Action<TClient, AsyncCallback, object> beginDelegate, string debugMessage, int numberOfRetries)
		{
			ArgumentValidator.ThrowIfNull("proxyPool", proxyPool);
			ArgumentValidator.ThrowIfNull("callerAsyncResult", callerAsyncResult);
			ArgumentValidator.ThrowIfNull("beginDelegate", beginDelegate);
			ArgumentValidator.ThrowIfZeroOrNegative("numberOfRetries", numberOfRetries);
			this.ProxyPool = proxyPool;
			this.CallerAsyncResult = callerAsyncResult;
			this.BeginDelegate = beginDelegate;
			this.DebugMessage = (debugMessage ?? string.Empty);
			this.NumberOfRetries = numberOfRetries;
		}

		protected TAsyncResult CallerAsyncResult { get; set; }

		private Action<TClient, AsyncCallback, object> BeginDelegate { get; set; }

		private string DebugMessage { get; set; }

		private int NumberOfRetries { get; set; }

		private IServiceProxyPool<TClient> ProxyPool { get; set; }

		protected void Begin()
		{
			Exception exception;
			if (!this.ProxyPool.TryCallServiceWithRetryAsyncBegin(new Action<IPooledServiceProxy<TClient>>(this.InvokeBeginDelegate), this.DebugMessage, this.NumberOfRetries, out exception))
			{
				TAsyncResult callerAsyncResult = this.CallerAsyncResult;
				callerAsyncResult.Complete(exception, false);
			}
		}

		protected abstract void InvokeEndDelegate(TClient proxy, IAsyncResult retryAsyncResult);

		private void InvokeBeginDelegate(IPooledServiceProxy<TClient> pooledProxy)
		{
			this.NumberOfRetries--;
			this.BeginDelegate(pooledProxy.Client, new AsyncCallback(this.RetryAsyncCallback), pooledProxy);
		}

		private void RetryAsyncCallback(IAsyncResult retryAsyncResult)
		{
			IPooledServiceProxy<TClient> cachedProxy = retryAsyncResult.AsyncState as IPooledServiceProxy<TClient>;
			Exception exception;
			bool flag = this.ProxyPool.TryCallServiceWithRetryAsyncEnd(cachedProxy, delegate(IPooledServiceProxy<TClient> x)
			{
				this.InvokeEndDelegate(x.Client, retryAsyncResult);
			}, this.DebugMessage, out exception);
			if (flag)
			{
				TAsyncResult callerAsyncResult = this.CallerAsyncResult;
				callerAsyncResult.Complete(retryAsyncResult.CompletedSynchronously);
				return;
			}
			if (this.NumberOfRetries > 0)
			{
				this.Begin();
				return;
			}
			TAsyncResult callerAsyncResult2 = this.CallerAsyncResult;
			callerAsyncResult2.Complete(exception, retryAsyncResult.CompletedSynchronously);
		}
	}
}
