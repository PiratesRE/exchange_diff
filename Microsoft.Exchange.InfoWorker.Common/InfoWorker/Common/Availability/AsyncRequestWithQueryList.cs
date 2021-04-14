using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class AsyncRequestWithQueryList : AsyncRequest
	{
		protected AsyncRequestWithQueryList(Application application, ClientContext clientContext, RequestType requestType, RequestLogger requestLogger, QueryList queryList) : base(application, clientContext, requestLogger)
		{
			this.QueryList = queryList;
			this.RequestType = requestType;
		}

		public RequestType RequestType { get; private set; }

		public QueryList QueryList { get; private set; }

		public override void Abort()
		{
			base.Abort();
			this.SetExceptionInResultList(AsyncRequestWithQueryList.TimeoutExpiredException);
		}

		protected void SetExceptionInResultList(LocalizedException exception)
		{
			AsyncRequestWithQueryList.RequestRoutingTracer.TraceDebug<object, LocalizedException>((long)this.GetHashCode(), "{0}: Setting exception to all queries: {1}", TraceContext.Get(), exception);
			this.QueryList.SetResultInAllQueries(base.Application.CreateQueryResult(exception));
		}

		private static readonly LocalizedException TimeoutExpiredException = new TimeoutExpiredException("Waiting-For-Request-Completion");

		private static readonly Trace RequestRoutingTracer = ExTraceGlobals.RequestRoutingTracer;
	}
}
