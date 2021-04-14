using System;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal interface IServiceProxyPool<TClient>
	{
		bool TryCallServiceWithRetryAsyncBegin(Action<IPooledServiceProxy<TClient>> action, string debugMessage, int numberOfRetries, out Exception exception);

		bool TryCallServiceWithRetryAsyncEnd(IPooledServiceProxy<TClient> cachedProxy, Action<IPooledServiceProxy<TClient>> action, string debugMessage, out Exception exception);
	}
}
