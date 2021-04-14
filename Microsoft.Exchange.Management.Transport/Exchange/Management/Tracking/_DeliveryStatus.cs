using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Management.Tracking
{
	public enum _DeliveryStatus
	{
		[TrackingStringsLocDescription(CoreStrings.IDs.StatusUnsuccessFul)]
		Unsuccessful,
		[TrackingStringsLocDescription(CoreStrings.IDs.StatusPending)]
		Pending,
		[TrackingStringsLocDescription(CoreStrings.IDs.StatusDelivered)]
		Delivered,
		[TrackingStringsLocDescription(CoreStrings.IDs.StatusTransferred)]
		Transferred,
		[TrackingStringsLocDescription(CoreStrings.IDs.StatusRead)]
		Read
	}
}
