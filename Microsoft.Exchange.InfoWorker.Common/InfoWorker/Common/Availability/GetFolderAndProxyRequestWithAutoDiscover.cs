using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class GetFolderAndProxyRequestWithAutoDiscover : AsyncRequestWithQueryList, IDisposable
	{
		public GetFolderAndProxyRequestWithAutoDiscover(Application application, InternalClientContext clientContext, RequestLogger requestLogger, QueryList queryList, TargetForestConfiguration targetForestConfiguration) : base(application, clientContext, RequestType.CrossForest, requestLogger, queryList)
		{
			this.targetForestConfiguration = targetForestConfiguration;
		}

		public void Dispose()
		{
			if (this.autoDiscoverQuery != null)
			{
				this.autoDiscoverQuery.Dispose();
			}
			if (this.dispatcher != null)
			{
				this.dispatcher.Dispose();
			}
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
			this.queryItems = AutoDiscoverQueryItem.CreateAutoDiscoverQueryItems(base.Application, base.QueryList, this.targetForestConfiguration.AutoDiscoverUrl);
			this.autoDiscoverQuery = new AutoDiscoverQueryInternal(base.Application, base.ClientContext, base.RequestLogger, this.targetForestConfiguration, this.queryItems, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestXmlByUser.Create), base.QueryList);
			this.autoDiscoverQuery.BeginInvoke(new TaskCompleteCallback(this.Complete1));
		}

		private void Complete1(AsyncTask task)
		{
			this.dispatcher = new DispatcherWithAutoDiscoverResults(base.Application, base.QueryList, this.queryItems, null, RequestType.CrossForest, new DispatcherWithAutoDiscoverResults.CreateRequestWithQueryListDelegate(this.CreateRequest));
			this.dispatcher.BeginInvoke(new TaskCompleteCallback(this.Complete2));
		}

		private AsyncRequestWithQueryList CreateRequest(QueryList queryList, ProxyAuthenticator proxyAuthenticator, WebServiceUri webServiceUri, UriSource source)
		{
			return new GetFolderAndProxyRequest(base.Application, (InternalClientContext)base.ClientContext, RequestType.CrossForest, base.RequestLogger, queryList, TargetServerVersion.Unknown, proxyAuthenticator, webServiceUri);
		}

		private void Complete2(AsyncTask task)
		{
			base.Complete();
		}

		public override string ToString()
		{
			return "GetFolderAndProxyRequestWithAutoDiscover for " + base.QueryList.Count + " mailboxes";
		}

		private TargetForestConfiguration targetForestConfiguration;

		private AutoDiscoverQuery autoDiscoverQuery;

		private AutoDiscoverQueryItem[] queryItems;

		private DispatcherWithAutoDiscoverResults dispatcher;
	}
}
