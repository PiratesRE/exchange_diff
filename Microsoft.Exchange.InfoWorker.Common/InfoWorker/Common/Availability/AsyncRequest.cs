using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class AsyncRequest : AsyncTask
	{
		protected AsyncRequest(Application application, ClientContext clientContext, RequestLogger requestLogger)
		{
			this.Application = application;
			this.ClientContext = clientContext;
			this.RequestLogger = requestLogger;
		}

		public ClientContext ClientContext { get; private set; }

		public Application Application { get; private set; }

		public RequestLogger RequestLogger { get; private set; }
	}
}
