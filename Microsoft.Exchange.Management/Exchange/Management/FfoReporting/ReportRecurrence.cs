using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Management.FfoReporting
{
	public enum ReportRecurrence
	{
		[LocDescription(CoreStrings.IDs.RunOnce)]
		RunOnce,
		[LocDescription(CoreStrings.IDs.RunWeekly)]
		Weekly,
		[LocDescription(CoreStrings.IDs.RunMonthly)]
		Monthly
	}
}
