using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal class RetriableAsyncOperation<TClient> : RetriableAsyncOperationBase<TClient, BasicAsyncResult>
	{
		private RetriableAsyncOperation(IServiceProxyPool<TClient> proxyPool, Action<TClient, AsyncCallback, object> beginDelegate, Action<TClient, IAsyncResult> endDelegate, AsyncCallback callerAsyncCallback, object callerState, string debugMessage, int numberOfRetries) : base(proxyPool, new BasicAsyncResult(callerAsyncCallback, callerState), beginDelegate, debugMessage, numberOfRetries)
		{
			ArgumentValidator.ThrowIfNull("endDelegate", endDelegate);
			this.EndDelegate = endDelegate;
		}

		public Action<TClient, IAsyncResult> EndDelegate { get; set; }

		internal static IAsyncResult Start(IServiceProxyPool<TClient> proxyPool, Action<TClient, AsyncCallback, object> beginDelegate, Action<TClient, IAsyncResult> endDelegate, AsyncCallback asyncCallback, object asyncState, string debugMessage, int numberOfRetries)
		{
			RetriableAsyncOperation<TClient> retriableAsyncOperation = new RetriableAsyncOperation<TClient>(proxyPool, beginDelegate, endDelegate, asyncCallback, asyncState, debugMessage, numberOfRetries);
			retriableAsyncOperation.Begin();
			return retriableAsyncOperation.CallerAsyncResult;
		}

		protected override void InvokeEndDelegate(TClient proxy, IAsyncResult retryAsyncResult)
		{
			this.EndDelegate(proxy, retryAsyncResult);
		}
	}
}
