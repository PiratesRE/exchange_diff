using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal class RetriableAsyncOperation<TClient, TResult> : RetriableAsyncOperationBase<TClient, BasicAsyncResult<TResult>>
	{
		private RetriableAsyncOperation(IServiceProxyPool<TClient> proxyPool, Action<TClient, AsyncCallback, object> beginDelegate, Func<TClient, IAsyncResult, TResult> endDelegate, AsyncCallback callerAsyncCallback, object callerState, string debugMessage, int numberOfRetries) : base(proxyPool, new BasicAsyncResult<TResult>(callerAsyncCallback, callerState), beginDelegate, debugMessage, numberOfRetries)
		{
			ArgumentValidator.ThrowIfNull("endDelegate", endDelegate);
			this.EndDelegate = endDelegate;
		}

		public Func<TClient, IAsyncResult, TResult> EndDelegate { get; set; }

		internal static IAsyncResult Start(IServiceProxyPool<TClient> proxyPool, Action<TClient, AsyncCallback, object> beginDelegate, Func<TClient, IAsyncResult, TResult> endDelegate, AsyncCallback asyncCallback, object asyncState, string debugMessage, int numberOfRetries)
		{
			RetriableAsyncOperation<TClient, TResult> retriableAsyncOperation = new RetriableAsyncOperation<TClient, TResult>(proxyPool, beginDelegate, endDelegate, asyncCallback, asyncState, debugMessage, numberOfRetries);
			retriableAsyncOperation.Begin();
			return retriableAsyncOperation.CallerAsyncResult;
		}

		protected override void InvokeEndDelegate(TClient proxy, IAsyncResult retryAsyncResult)
		{
			BasicAsyncResult<TResult> callerAsyncResult = base.CallerAsyncResult;
			callerAsyncResult.Result = this.EndDelegate(proxy, retryAsyncResult);
		}
	}
}
