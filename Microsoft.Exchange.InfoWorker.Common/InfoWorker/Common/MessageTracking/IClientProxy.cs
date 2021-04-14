using System;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal interface IClientProxy
	{
		string TargetInfoForLogging { get; }

		string TargetInfoForDisplay { get; }

		FindMessageTrackingReportResponseMessageType FindMessageTrackingReport(FindMessageTrackingReportRequestTypeWrapper request, TimeSpan timeout);

		InternalGetMessageTrackingReportResponse GetMessageTrackingReport(GetMessageTrackingReportRequestTypeWrapper request, TimeSpan timeout);
	}
}
