using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class DispatcherWithAutoDiscoverResults : BaseRequestDispatcher
	{
		public DispatcherWithAutoDiscoverResults(Application application, QueryList queryList, IList<AutoDiscoverQueryItem> autoDiscoverQueryItems, ProxyAuthenticator proxyAuthenticator, RequestType requestType, DispatcherWithAutoDiscoverResults.CreateRequestWithQueryListDelegate createRequestDelegate)
		{
			DispatcherWithAutoDiscoverResults <>4__this = this;
			this.createRequestDelegate = createRequestDelegate;
			for (int i = 0; i < queryList.Count; i++)
			{
				BaseQuery baseQuery = queryList[i];
				AutoDiscoverResult autoDiscoverResult = autoDiscoverQueryItems[i].Result;
				if (autoDiscoverResult.Exception != null)
				{
					DispatcherWithAutoDiscoverResults.RequestRoutingTracer.TraceError<object, EmailAddress>((long)this.GetHashCode(), "{0}: autodiscover for {1} failed and it will not be dispatched for query", TraceContext.Get(), baseQuery.Email);
					baseQuery.SetResultOnFirstCall(application.CreateQueryResult(autoDiscoverResult.Exception));
				}
				else
				{
					string key = autoDiscoverResult.WebServiceUri.Uri.ToString();
					if (autoDiscoverResult.WebServiceUri.EmailAddress != null)
					{
						baseQuery.RecipientData.EmailAddress = autoDiscoverResult.WebServiceUri.EmailAddress;
					}
					base.Add(key, baseQuery, requestType, (QueryList perRequestQueryList) => <>4__this.createRequestDelegate(perRequestQueryList, proxyAuthenticator ?? autoDiscoverResult.ProxyAuthenticator, autoDiscoverResult.WebServiceUri, UriSource.EmailDomain));
				}
			}
		}

		private DispatcherWithAutoDiscoverResults.CreateRequestWithQueryListDelegate createRequestDelegate;

		private static readonly Trace RequestRoutingTracer = ExTraceGlobals.RequestRoutingTracer;

		public delegate AsyncRequest CreateRequestWithQueryListDelegate(QueryList queryList, ProxyAuthenticator proxyAuthenticator, WebServiceUri webServiceUri, UriSource source);
	}
}
