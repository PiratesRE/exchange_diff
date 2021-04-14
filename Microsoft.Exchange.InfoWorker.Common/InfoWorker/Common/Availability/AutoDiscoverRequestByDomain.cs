using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class AutoDiscoverRequestByDomain : AutoDiscoverRequestOperation
	{
		public static AutoDiscoverRequestOperation CreateForCrossForest(Application application, ClientContext clientContext, RequestLogger requestLogger, Uri targetUri, AutoDiscoverAuthenticator authenticator, EmailAddress[] emailAddresses, UriSource uriSource, AutodiscoverType autodiscoverType)
		{
			return new AutoDiscoverRequestByDomain(application, clientContext, requestLogger, new AutoDiscoverAuthenticator(ProxyAuthenticator.CreateForSoap(clientContext.MessageId)), targetUri, emailAddresses, autodiscoverType);
		}

		public static AutoDiscoverRequestOperation CreateForExternal(Application application, ClientContext clientContext, RequestLogger requestLogger, Uri targetUri, AutoDiscoverAuthenticator authenticator, EmailAddress[] emailAddresses, UriSource uriSource, AutodiscoverType autodiscoverType)
		{
			return new AutoDiscoverRequestByDomain(application, clientContext, requestLogger, authenticator, targetUri, emailAddresses, autodiscoverType);
		}

		private AutoDiscoverRequestByDomain(Application application, ClientContext clientContext, RequestLogger requestLogger, AutoDiscoverAuthenticator authenticator, Uri targetUri, EmailAddress[] emailAddresses, AutodiscoverType autodiscoverType) : base(application, clientContext, requestLogger, targetUri, authenticator, emailAddresses, autodiscoverType)
		{
		}

		public override void Abort()
		{
			base.Abort();
			if (this.domainSoapAutoDiscoverRequest != null)
			{
				this.domainSoapAutoDiscoverRequest.Abort();
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			if (this.domainSoapAutoDiscoverRequest != null)
			{
				this.domainSoapAutoDiscoverRequest.Dispose();
			}
		}

		public override void BeginInvoke(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			this.domainSoapAutoDiscoverRequest = new DomainSoapAutoDiscoverRequest(base.Application, base.ClientContext, base.RequestLogger, base.Authenticator, base.TargetUri, base.EmailAddresses, base.AutodiscoverType);
			this.domainSoapAutoDiscoverRequest.BeginInvoke(new TaskCompleteCallback(this.CompleteRequest));
		}

		private void CompleteRequest(AsyncTask task)
		{
			base.HandleResultsAndCompleteRequest(this.domainSoapAutoDiscoverRequest);
		}

		private DomainSoapAutoDiscoverRequest domainSoapAutoDiscoverRequest;
	}
}
