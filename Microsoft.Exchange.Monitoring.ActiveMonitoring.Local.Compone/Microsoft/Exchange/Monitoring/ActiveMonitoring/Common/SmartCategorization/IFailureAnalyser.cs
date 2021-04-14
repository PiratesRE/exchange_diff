using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common.SmartCategorization
{
	internal interface IFailureAnalyser
	{
		FailureDetails Analyse(RequestContext requestContext, RequestFailureContext requestFailureContext);
	}
}
