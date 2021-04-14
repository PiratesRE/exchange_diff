using System;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class WSGetResult
	{
		internal MessageTrackingReportType Report;

		internal RecipientEventData RecipientEventData;
	}
}
