using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class ExternalByOAuthProxyWebRequestWithAutoDiscover : AsyncRequestWithQueryList
	{
		public ExternalByOAuthProxyWebRequestWithAutoDiscover(Application application, InternalClientContext clientContext, RequestLogger requestLogger, QueryList queryList, Uri autoDiscoverUrl, CreateAutoDiscoverRequestDelegate createAutoDiscoverRequest) : base(application, clientContext, RequestType.FederatedCrossForest, requestLogger, queryList)
		{
			this.autoDiscoverUrl = autoDiscoverUrl;
			this.createAutoDiscoverRequest = createAutoDiscoverRequest;
		}

		public override void Abort()
		{
			base.Abort();
			if (this.autoDiscoverQuery != null)
			{
				this.autoDiscoverQuery.Abort();
			}
			if (this.dispatcher != null)
			{
				this.dispatcher.Abort();
			}
		}

		public override void BeginInvoke(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			ProxyAuthenticator proxyAuthenticator = ProxyAuthenticator.Create(OAuthCredentialsFactory.CreateAsApp(base.ClientContext as InternalClientContext, base.RequestLogger), base.ClientContext.MessageId, true);
			this.queryItems = AutoDiscoverQueryItem.CreateAutoDiscoverQueryItems(base.Application, base.QueryList, this.autoDiscoverUrl);
			this.autoDiscoverQuery = new AutoDiscoverQueryExternalByOAuth(base.Application, base.ClientContext, base.RequestLogger, this.autoDiscoverUrl, proxyAuthenticator, this.queryItems, this.createAutoDiscoverRequest, base.QueryList);
			this.autoDiscoverQuery.BeginInvoke(new TaskCompleteCallback(this.Complete1));
		}

		private void Complete1(AsyncTask task)
		{
			ProxyAuthenticator proxyAuthenticator = ProxyAuthenticator.Create(OAuthCredentialsFactory.Create(base.ClientContext as InternalClientContext, base.RequestLogger), base.ClientContext.MessageId, false);
			this.dispatcher = new DispatcherWithAutoDiscoverResults(base.Application, base.QueryList, this.queryItems, proxyAuthenticator, RequestType.FederatedCrossForest, new DispatcherWithAutoDiscoverResults.CreateRequestWithQueryListDelegate(this.CreateRequest));
			this.dispatcher.BeginInvoke(new TaskCompleteCallback(this.Complete2));
		}

		private AsyncRequest CreateRequest(QueryList queryList, ProxyAuthenticator proxyAuthenticator, WebServiceUri webServiceUri, UriSource source)
		{
			return new ProxyWebRequest(base.Application, base.ClientContext, RequestType.FederatedCrossForest, base.RequestLogger, queryList, TargetServerVersion.Unknown, proxyAuthenticator, webServiceUri, source);
		}

		private void Complete2(AsyncTask task)
		{
			base.Complete();
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"ExternalByOAuthProxyWebRequestWithAutoDiscover for ",
				base.QueryList.Count,
				" mailboxes to ",
				this.autoDiscoverUrl
			});
		}

		private AutoDiscoverQueryItem[] queryItems;

		private AutoDiscoverQuery autoDiscoverQuery;

		private DispatcherWithAutoDiscoverResults dispatcher;

		private Uri autoDiscoverUrl;

		private CreateAutoDiscoverRequestDelegate createAutoDiscoverRequest;
	}
}
