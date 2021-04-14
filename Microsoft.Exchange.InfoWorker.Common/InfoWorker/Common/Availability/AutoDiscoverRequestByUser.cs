using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class AutoDiscoverRequestByUser : AutoDiscoverRequestOperation
	{
		public static AutoDiscoverRequestOperation Create(Application application, ClientContext clientContext, RequestLogger requestLogger, Uri targetUri, AutoDiscoverAuthenticator authenticator, EmailAddress[] emailAddresses, UriSource uriSource, AutodiscoverType autodiscoverType)
		{
			return new AutoDiscoverRequestByUser(application, clientContext, requestLogger, targetUri, authenticator, emailAddresses, autodiscoverType);
		}

		private AutoDiscoverRequestByUser(Application application, ClientContext clientContext, RequestLogger requestLogger, Uri targetUri, AutoDiscoverAuthenticator authenticator, EmailAddress[] emailAddresses, AutodiscoverType autodiscoverType) : base(application, clientContext, requestLogger, targetUri, authenticator, emailAddresses, autodiscoverType)
		{
		}

		public override void Abort()
		{
			base.Abort();
			if (this.request != null)
			{
				this.request.Abort();
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			if (this.request != null)
			{
				this.request.Dispose();
			}
		}

		public override void BeginInvoke(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			this.request = new UserSoapAutoDiscoverRequest(base.Application, base.ClientContext, RequestType.FederatedCrossForest, base.RequestLogger, base.Authenticator, base.TargetUri, base.EmailAddresses, base.AutodiscoverType);
			this.request.BeginInvoke(new TaskCompleteCallback(this.CompleteRequest));
		}

		private void CompleteRequest(AsyncTask task)
		{
			base.HandleResultsAndCompleteRequest(this.request);
		}

		private UserSoapAutoDiscoverRequest request;
	}
}
