using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Management.FfoReporting
{
	public enum MessageDeliveryStatus
	{
		[LocDescription(CoreStrings.IDs.DeliveryStatusAll)]
		All,
		[LocDescription(CoreStrings.IDs.DeliveryStatusDelivered)]
		Delivered,
		[LocDescription(CoreStrings.IDs.DeliveryStatusFailed)]
		Failed,
		[LocDescription(CoreStrings.IDs.DeliveryStatusExpanded)]
		Expanded
	}
}
