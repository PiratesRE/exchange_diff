using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class GetMessageTrackingBaseQueryResult : BaseQueryResult
	{
		public GetMessageTrackingReportResponseMessageType Response { get; internal set; }

		internal GetMessageTrackingBaseQueryResult()
		{
		}

		internal GetMessageTrackingBaseQueryResult(LocalizedException exception) : base(exception)
		{
		}
	}
}
