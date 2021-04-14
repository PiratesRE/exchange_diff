using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class ProxyWebRequestWithAutoDiscover : AsyncRequestWithQueryList, IDisposable
	{
		public ProxyWebRequestWithAutoDiscover(Application application, ClientContext clientContext, RequestLogger requestLogger, QueryList queryList, TargetForestConfiguration targetForestConfiguration, CreateAutoDiscoverRequestDelegate createAutoDiscoverRequest) : base(application, clientContext, RequestType.CrossForest, requestLogger, queryList)
		{
			this.targetForestConfiguration = targetForestConfiguration;
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

		public override void BeginInvoke(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			this.queryItems = AutoDiscoverQueryItem.CreateAutoDiscoverQueryItems(base.Application, base.QueryList, this.targetForestConfiguration.AutoDiscoverUrl);
			this.autoDiscoverQuery = new AutoDiscoverQueryInternal(base.Application, base.ClientContext, base.RequestLogger, this.targetForestConfiguration, this.queryItems, this.createAutoDiscoverRequest, base.QueryList);
			this.requestTimer = Stopwatch.StartNew();
			this.autoDiscoverQuery.BeginInvoke(new TaskCompleteCallback(this.Complete1));
		}

		private void Complete1(AsyncTask task)
		{
			this.requestTimer.Stop();
			if (!base.Aborted)
			{
				base.QueryList.LogLatency("PWRADC1", this.requestTimer.ElapsedMilliseconds);
			}
			this.dispatcher = new DispatcherWithAutoDiscoverResults(base.Application, base.QueryList, this.queryItems, null, RequestType.CrossForest, new DispatcherWithAutoDiscoverResults.CreateRequestWithQueryListDelegate(this.CreateRequest));
			this.dispatcher.BeginInvoke(new TaskCompleteCallback(this.Complete2));
		}

		private AsyncRequest CreateRequest(QueryList queryList, ProxyAuthenticator proxyAuthenticator, WebServiceUri webServiceUri, UriSource source)
		{
			return new ProxyWebRequest(base.Application, base.ClientContext, RequestType.CrossForest, base.RequestLogger, queryList, TargetServerVersion.Unknown, proxyAuthenticator, webServiceUri, source);
		}

		private void Complete2(AsyncTask task)
		{
			base.Complete();
		}

		public override string ToString()
		{
			return "ProxyWebRequestWithAutoDiscover for " + base.QueryList.Count + " mailboxes";
		}

		private const string ProxyWebRequestWithAutoDiscoveryComplete1Marker = "PWRADC1";

		private TargetForestConfiguration targetForestConfiguration;

		private AutoDiscoverQuery autoDiscoverQuery;

		private AutoDiscoverQueryItem[] queryItems;

		private DispatcherWithAutoDiscoverResults dispatcher;

		private CreateAutoDiscoverRequestDelegate createAutoDiscoverRequest;

		private Stopwatch requestTimer;

		private static readonly Microsoft.Exchange.Diagnostics.Trace RequestRoutingTracer = ExTraceGlobals.RequestRoutingTracer;
	}
}
