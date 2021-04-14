using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Management.FfoReporting
{
	public enum ReportSeverity
	{
		[LocDescription(CoreStrings.IDs.ReportSeverityLow)]
		Low,
		[LocDescription(CoreStrings.IDs.ReportSeverityMedium)]
		Medium,
		[LocDescription(CoreStrings.IDs.ReportSeverityHigh)]
		High
	}
}
