using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common.SmartCategorization
{
	internal abstract class FailureAnalyserBase : IFailureAnalyser
	{
		public FailureDetails Analyse(RequestContext requestContext, RequestFailureContext requestFailureContext)
		{
			if (requestFailureContext == null)
			{
				throw new ArgumentNullException("requestFailureContext");
			}
			switch (requestFailureContext.FailurePoint)
			{
			case RequestFailureContext.RequestFailurePoint.FrontEnd:
				return this.AnalyseCafeFailure(requestFailureContext);
			case RequestFailureContext.RequestFailurePoint.BackEnd:
				return this.AnalyseBackendFailure(requestFailureContext);
			default:
				return new FailureDetails();
			}
		}

		protected virtual FailureDetails AnalyseCafeFailure(RequestFailureContext requestFailureContext)
		{
			return new FailureDetails();
		}

		protected virtual FailureDetails AnalyseBackendFailure(RequestFailureContext requestFailureContext)
		{
			return new FailureDetails();
		}
	}
}
