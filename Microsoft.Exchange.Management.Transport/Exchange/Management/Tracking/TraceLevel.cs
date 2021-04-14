using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Management.Tracking
{
	public enum TraceLevel
	{
		[TrackingStringsLocDescription(CoreStrings.IDs.TraceLevelLow)]
		Low,
		[TrackingStringsLocDescription(CoreStrings.IDs.TraceLevelMedium)]
		Medium,
		[TrackingStringsLocDescription(CoreStrings.IDs.TraceLevelHigh)]
		High
	}
}
