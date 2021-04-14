using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class WatsonTroubleshootingReport : WatsonGenericReport
	{
		internal WatsonTroubleshootingReport(string eventType, string triggerVersion, string locationIdentity, string exceptionName, string callstack, string methodName, string detailedExceptionInformation, string traceFileName) : base(eventType, WatsonReport.GetValidString(triggerVersion), ExWatson.AppName, WatsonReport.ExchangeFormattedVersion(ExWatson.ApplicationVersion), WatsonReport.GetValidString(locationIdentity), WatsonReport.GetValidString(exceptionName), WatsonReport.GetValidString(callstack), WatsonGenericReport.StringHashFromStackTrace(WatsonReport.GetValidString(callstack)), WatsonReport.GetValidString(methodName), detailedExceptionInformation)
		{
			this.traceFileName = traceFileName;
		}

		protected override WatsonIssueType GetIssueTypeCode()
		{
			return WatsonIssueType.ManagedCodeTroubleshootingIssue;
		}

		protected override void AddExtraFiles(WerSafeHandle watsonReportHandle)
		{
			DiagnosticsNativeMethods.WerReportAddFile(watsonReportHandle, this.traceFileName, DiagnosticsNativeMethods.WER_FILE_TYPE.WerFileTypeOther, (DiagnosticsNativeMethods.WER_FILE_FLAGS)0U);
		}

		private string traceFileName;
	}
}
