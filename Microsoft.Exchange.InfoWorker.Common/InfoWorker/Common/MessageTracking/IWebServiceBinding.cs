using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal interface IWebServiceBinding
	{
		InternalGetMessageTrackingReportResponse GetMessageTrackingReport(string messageTrackingReportId, ReportTemplate reportTemplate, SmtpAddress[] recipientFilter, SearchScope scope, bool returnQueueEvents, TrackingEventBudget eventBudget);

		FindMessageTrackingReportResponseMessageType FindMessageTrackingReport(string domain, SmtpAddress? senderAddress, SmtpAddress? recipientAddress, string serverHint, SmtpAddress? federatedDeliveryMailbox, SearchScope scope, string messageId, string subject, bool expandTree, bool searchAsRecip, bool searchForModerationResult, DateTime start, DateTime end, TrackingEventBudget eventBudget);

		string TargetInfoForDisplay { get; }
	}
}
