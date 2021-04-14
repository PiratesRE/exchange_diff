using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class ExternalProxyWebRequest : AsyncRequestWithQueryList
	{
		public ExternalProxyWebRequest(Application application, ClientContext clientContext, RequestLogger requestLogger, QueryList queryList, ExternalAuthenticationRequest externalAuthenticationRequest, WebServiceUri webServiceUri, SmtpAddress sharingKey) : base(application, clientContext, RequestType.FederatedCrossForest, requestLogger, queryList)
		{
			this.externalAuthenticationRequest = externalAuthenticationRequest;
			this.webServiceUri = webServiceUri;
			this.sharingKey = sharingKey;
		}

		public override void Abort()
		{
			base.Abort();
			this.externalAuthenticationRequest.Abort();
			if (this.proxyWebRequest != null)
			{
				this.proxyWebRequest.Abort();
			}
		}

		public override void BeginInvoke(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.externalAuthenticationRequest.BeginInvoke(new TaskCompleteCallback(this.CompleteAuthenticator));
			stopwatch.Stop();
			base.QueryList.LogLatency("EPRBI", stopwatch.ElapsedMilliseconds);
		}

		private void CompleteAuthenticator(AsyncTask task)
		{
			if (this.externalAuthenticationRequest.Exception != null)
			{
				base.SetExceptionInResultList(this.externalAuthenticationRequest.Exception);
				base.Complete();
				return;
			}
			ProxyAuthenticator proxyAuthenticator = ProxyAuthenticator.Create(this.externalAuthenticationRequest.RequestedToken, this.sharingKey, base.ClientContext.MessageId);
			this.proxyWebRequest = new ProxyWebRequest(base.Application, base.ClientContext, RequestType.FederatedCrossForest, base.RequestLogger, base.QueryList, TargetServerVersion.Unknown, proxyAuthenticator, this.webServiceUri, this.webServiceUri.Source);
			this.proxyWebRequest.BeginInvoke(new TaskCompleteCallback(this.CompleteWebRequest));
		}

		private void CompleteWebRequest(AsyncTask task)
		{
			base.Complete();
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"ExternalProxyWebRequest for ",
				base.QueryList.Count,
				" mailboxes at ",
				this.webServiceUri.Uri.OriginalString,
				" with ",
				this.externalAuthenticationRequest.ToString()
			});
		}

		public const string ExternalProxyWebReqeustBeginInvokeMarker = "EPRBI";

		private ExternalAuthenticationRequest externalAuthenticationRequest;

		private ProxyWebRequest proxyWebRequest;

		private WebServiceUri webServiceUri;

		private SmtpAddress sharingKey;
	}
}
