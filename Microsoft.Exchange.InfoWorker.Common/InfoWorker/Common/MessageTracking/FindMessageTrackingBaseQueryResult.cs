using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class FindMessageTrackingBaseQueryResult : BaseQueryResult
	{
		public FindMessageTrackingReportResponseMessageType Response { get; internal set; }

		internal FindMessageTrackingBaseQueryResult()
		{
		}

		internal FindMessageTrackingBaseQueryResult(LocalizedException exception) : base(exception)
		{
		}
	}
}
