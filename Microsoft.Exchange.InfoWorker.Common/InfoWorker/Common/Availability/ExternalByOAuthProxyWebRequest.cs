using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class ExternalByOAuthProxyWebRequest : AsyncRequestWithQueryList
	{
		public ExternalByOAuthProxyWebRequest(Application application, ClientContext clientContext, RequestLogger requestLogger, QueryList queryList, WebServiceUri webServiceUri) : base(application, clientContext, RequestType.FederatedCrossForest, requestLogger, queryList)
		{
			this.webServiceUri = webServiceUri;
		}

		public override void Abort()
		{
			base.Abort();
			if (this.proxyWebRequest != null)
			{
				this.proxyWebRequest.Abort();
			}
		}

		public override void BeginInvoke(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			ProxyAuthenticator proxyAuthenticator = ProxyAuthenticator.Create(OAuthCredentialsFactory.Create(base.ClientContext as InternalClientContext, base.RequestLogger), base.ClientContext.MessageId, false);
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
				"ExternalByOAuthProxyWebRequest for ",
				base.QueryList.Count,
				" mailboxes at ",
				this.webServiceUri.Uri.OriginalString
			});
		}

		private ProxyWebRequest proxyWebRequest;

		private readonly WebServiceUri webServiceUri;
	}
}
