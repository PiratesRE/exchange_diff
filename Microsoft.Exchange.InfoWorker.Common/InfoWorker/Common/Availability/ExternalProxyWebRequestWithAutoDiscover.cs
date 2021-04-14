using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class ExternalProxyWebRequestWithAutoDiscover : AsyncRequestWithQueryList
	{
		public ExternalProxyWebRequestWithAutoDiscover(Application application, InternalClientContext clientContext, RequestLogger requestLogger, QueryList queryList, ExternalAuthenticationRequest autoDiscoverExternalAuthenticationRequest, ExternalAuthenticationRequest webProxyExternalAuthenticationRequest, Uri autoDiscoverUrl, SmtpAddress sharingKey, CreateAutoDiscoverRequestDelegate createAutoDiscoverRequest) : base(application, clientContext, RequestType.FederatedCrossForest, requestLogger, queryList)
		{
			this.autoDiscoverExternalAuthenticationRequest = autoDiscoverExternalAuthenticationRequest;
			this.webProxyExternalAuthenticationRequest = webProxyExternalAuthenticationRequest;
			this.autoDiscoverUrl = autoDiscoverUrl;
			this.sharingKey = sharingKey;
			this.createAutoDiscoverRequest = createAutoDiscoverRequest;
		}

		public override void Abort()
		{
			base.Abort();
			if (this.parallel != null)
			{
				this.parallel.Abort();
			}
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
			this.parallel = new AsyncTaskParallel(new AsyncTask[]
			{
				this.autoDiscoverExternalAuthenticationRequest,
				this.webProxyExternalAuthenticationRequest
			});
			this.requestTimer = Stopwatch.StartNew();
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.parallel.BeginInvoke(new TaskCompleteCallback(this.Complete1));
			stopwatch.Stop();
			base.QueryList.LogLatency("EPWRADBI", stopwatch.ElapsedMilliseconds);
		}

		private void Complete1(AsyncTask task)
		{
			this.requestTimer.Stop();
			if (!base.Aborted)
			{
				base.QueryList.LogLatency("EPWRADC1", this.requestTimer.ElapsedMilliseconds);
			}
			if (this.autoDiscoverExternalAuthenticationRequest.Exception != null)
			{
				base.SetExceptionInResultList(this.autoDiscoverExternalAuthenticationRequest.Exception);
				base.Complete();
				return;
			}
			if (this.webProxyExternalAuthenticationRequest.Exception != null)
			{
				base.SetExceptionInResultList(this.webProxyExternalAuthenticationRequest.Exception);
				base.Complete();
				return;
			}
			ProxyAuthenticator proxyAuthenticator = ProxyAuthenticator.Create(this.autoDiscoverExternalAuthenticationRequest.RequestedToken, this.sharingKey, base.ClientContext.MessageId);
			this.queryItems = new AutoDiscoverQueryItem[base.QueryList.Count];
			string target = this.autoDiscoverUrl.ToString();
			for (int i = 0; i < base.QueryList.Count; i++)
			{
				base.QueryList[i].Target = target;
				this.queryItems[i] = new AutoDiscoverQueryItem(base.QueryList[i].RecipientData, base.Application.Name, base.QueryList[i]);
			}
			this.autoDiscoverQuery = new AutoDiscoverQueryExternal(base.Application, base.ClientContext, base.RequestLogger, this.autoDiscoverUrl, proxyAuthenticator, this.queryItems, this.createAutoDiscoverRequest, base.QueryList);
			this.autoDiscoverQuery.BeginInvoke(new TaskCompleteCallback(this.Complete2));
		}

		private void Complete2(AsyncTask task)
		{
			ProxyAuthenticator proxyAuthenticator = ProxyAuthenticator.Create(this.webProxyExternalAuthenticationRequest.RequestedToken, this.sharingKey, base.ClientContext.MessageId);
			this.dispatcher = new DispatcherWithAutoDiscoverResults(base.Application, base.QueryList, this.queryItems, proxyAuthenticator, RequestType.FederatedCrossForest, new DispatcherWithAutoDiscoverResults.CreateRequestWithQueryListDelegate(this.CreateRequest));
			this.dispatcher.BeginInvoke(new TaskCompleteCallback(this.Complete3));
		}

		private AsyncRequest CreateRequest(QueryList queryList, ProxyAuthenticator proxyAuthenticator, WebServiceUri webServiceUri, UriSource source)
		{
			return new ProxyWebRequest(base.Application, base.ClientContext, RequestType.FederatedCrossForest, base.RequestLogger, queryList, TargetServerVersion.Unknown, proxyAuthenticator, webServiceUri, source);
		}

		private void Complete3(AsyncTask task)
		{
			base.Complete();
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"ExternalProxyWebRequestWithAutoDiscover for ",
				base.QueryList.Count,
				" mailboxes to ",
				this.autoDiscoverUrl
			});
		}

		public const string ExternalProxyWebRequestWithAutoDiscoverBeginInvokeMarker = "EPWRADBI";

		public const string ExternalProxyWebRequestWithAutoDiscoverComplete1Marker = "EPWRADC1";

		private ExternalAuthenticationRequest autoDiscoverExternalAuthenticationRequest;

		private ExternalAuthenticationRequest webProxyExternalAuthenticationRequest;

		private AutoDiscoverQueryItem[] queryItems;

		private AutoDiscoverQuery autoDiscoverQuery;

		private DispatcherWithAutoDiscoverResults dispatcher;

		private AsyncTaskParallel parallel;

		private SmtpAddress sharingKey;

		private Uri autoDiscoverUrl;

		private CreateAutoDiscoverRequestDelegate createAutoDiscoverRequest;

		private Stopwatch requestTimer;
	}
}
