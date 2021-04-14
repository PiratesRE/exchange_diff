using System;

namespace Microsoft.Exchange.Data.ThrottlingService.Client
{
	internal interface IThrottlingClientPerformanceCounters
	{
		void AddRequestStatus(ThrottlingRpcResult result);

		void AddRequestStatus(ThrottlingRpcResult result, long requestTimeMsec);
	}
}
