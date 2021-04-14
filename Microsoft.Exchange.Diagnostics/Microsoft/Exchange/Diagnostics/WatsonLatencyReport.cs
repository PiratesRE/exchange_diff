using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class WatsonLatencyReport : WatsonGenericReport
	{
		public WatsonLatencyReport(string eventType, string triggerVersion, string locationIdentity, string exceptionName, string callstack, string methodName, string detailedExceptionInformation) : base(eventType, WatsonReport.GetValidString(triggerVersion), ExWatson.AppName, WatsonReport.ExchangeFormattedVersion(ExWatson.ApplicationVersion), WatsonReport.GetValidString(locationIdentity), WatsonReport.GetValidString(exceptionName), WatsonReport.GetValidString(callstack), WatsonGenericReport.StringHashFromStackTrace(WatsonReport.GetValidString(callstack)), WatsonReport.GetValidString(methodName), detailedExceptionInformation)
		{
		}

		protected override WatsonIssueType GetIssueTypeCode()
		{
			return WatsonIssueType.ManagedCodeLatencyIssue;
		}
	}
}
