using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class AutoDiscoverRequestXmlByUser : AutoDiscoverRequestOperation
	{
		public static AutoDiscoverRequestOperation Create(Application application, ClientContext clientContext, RequestLogger requestLogger, Uri targetUri, AutoDiscoverAuthenticator authenticator, EmailAddress[] emailAddresses, UriSource uriSource, AutodiscoverType autodiscoverType)
		{
			return new AutoDiscoverRequestXmlByUser(application, clientContext, requestLogger, targetUri, authenticator, emailAddresses, uriSource, autodiscoverType);
		}

		private AutoDiscoverRequestXmlByUser(Application application, ClientContext clientContext, RequestLogger requestLogger, Uri targetUri, AutoDiscoverAuthenticator authenticator, EmailAddress[] emailAddresses, UriSource uriSource, AutodiscoverType autodiscoverType) : base(application, clientContext, requestLogger, targetUri, authenticator, emailAddresses, autodiscoverType)
		{
			this.uriSource = uriSource;
		}

		public override void Abort()
		{
			base.Abort();
			if (this.parallel != null)
			{
				this.parallel.Abort();
			}
		}

		public override void BeginInvoke(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			this.requests = new AutoDiscoverRequest[base.EmailAddresses.Length];
			for (int i = 0; i < base.EmailAddresses.Length; i++)
			{
				this.requests[i] = this.GetRequest(base.EmailAddresses[i]);
			}
			this.parallel = new AsyncTaskParallel(this.requests);
			this.parallel.BeginInvoke(new TaskCompleteCallback(this.CompleteRequest));
		}

		private void CompleteRequest(AsyncTask task)
		{
			AutoDiscoverRequestResult[] array = new AutoDiscoverRequestResult[this.requests.Length];
			for (int i = 0; i < this.requests.Length; i++)
			{
				array[i] = this.GetResult(this.requests[i]);
			}
			base.HandleResultsAndCompleteRequest(null, array);
		}

		private AutoDiscoverRequest GetRequest(EmailAddress emailAddress)
		{
			return new AutoDiscoverRequest(base.Application, base.ClientContext, base.RequestLogger, base.TargetUri, emailAddress, base.Authenticator.Credentials, this.uriSource);
		}

		private AutoDiscoverRequestResult GetResult(AutoDiscoverRequest request)
		{
			return request.Result;
		}

		private UriSource uriSource;

		private AutoDiscoverRequest[] requests;

		private AsyncTask parallel;
	}
}
