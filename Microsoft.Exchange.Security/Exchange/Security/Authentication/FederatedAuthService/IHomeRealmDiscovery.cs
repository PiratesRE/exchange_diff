using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal interface IHomeRealmDiscovery
	{
		IAsyncResult StartRequestChain(object user, AsyncCallback callback, object state);

		IAsyncResult ProcessRequest(IAsyncResult asyncResult, AsyncCallback callback, object state);

		DomainConfig ProcessResponse(IAsyncResult asyncResult);

		string GetLatency();

		void Abort();

		LiveIdInstanceType Instance { get; }

		string RealmDiscoveryUri { get; }

		string ErrorString { get; }

		string LiveServer { get; }

		string StsTag { get; }

		long Latency { get; }

		long SSLConnectionLatency { get; }
	}
}
