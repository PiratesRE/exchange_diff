using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class AutoDiscoverRequestOperation : AsyncRequest, IDisposable
	{
		public Uri TargetUri { get; private set; }

		public AutoDiscoverAuthenticator Authenticator { get; private set; }

		public EmailAddress[] EmailAddresses { get; private set; }

		public Exception Exception { get; private set; }

		public string FrontEndServerName { get; private set; }

		public string BackEndServerName { get; private set; }

		public AutoDiscoverRequestResult[] Results { get; private set; }

		public AutodiscoverType AutodiscoverType { get; private set; }

		internal AutoDiscoverRequestOperation(Application application, ClientContext clientContext, RequestLogger requestLogger, Uri targetUri, AutoDiscoverAuthenticator authenticator, EmailAddress[] emailAddresses, AutodiscoverType autodiscoverType) : base(application, clientContext, requestLogger)
		{
			this.TargetUri = targetUri;
			this.Authenticator = authenticator;
			this.EmailAddresses = emailAddresses;
			this.AutodiscoverType = autodiscoverType;
		}

		public virtual void Dispose()
		{
		}

		protected void HandleResultsAndCompleteRequest(SoapAutoDiscoverRequest request)
		{
			this.FrontEndServerName = request.FrontEndServerName;
			this.BackEndServerName = request.BackEndServerName;
			this.HandleResultsAndCompleteRequest(request.Exception, request.Results);
		}

		protected void HandleResultsAndCompleteRequest(Exception exception, AutoDiscoverRequestResult[] results)
		{
			try
			{
				this.Exception = exception;
				this.Results = results;
			}
			finally
			{
				base.Complete();
			}
		}

		protected static readonly Trace AutoDiscoverTracer = ExTraceGlobals.AutoDiscoverTracer;
	}
}
