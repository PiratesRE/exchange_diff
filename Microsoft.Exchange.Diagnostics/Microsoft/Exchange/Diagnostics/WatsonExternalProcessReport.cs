using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Microsoft.Exchange.Diagnostics
{
	internal class WatsonExternalProcessReport : WatsonExceptionReport
	{
		public WatsonExternalProcessReport(Process process, string eventType, Exception exception, string detailedExceptionInformation, ReportOptions reportOptions) : base(eventType, process, exception, reportOptions)
		{
			this.callstack = exception.StackTrace;
			this.detailedExceptionInformation = detailedExceptionInformation;
		}

		protected override ProcSafeHandle GetProcessHandle()
		{
			if (base.IsProcessValid)
			{
				return base.GetProcessHandle();
			}
			return new ProcSafeHandle();
		}

		protected override void WriteSpecializedPartOfTextReport(TextWriter reportFile)
		{
			base.WriteReportFileHeader(reportFile, "Manifest Report");
			reportFile.WriteLine("P0(appVersion)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonExceptionReport.BucketParamId>(WatsonExceptionReport.BucketParamId.AppVersion)));
			reportFile.WriteLine("P1(appName)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonExceptionReport.BucketParamId>(WatsonExceptionReport.BucketParamId.AppName)));
			reportFile.WriteLine("P2(exMethodName)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonExceptionReport.BucketParamId>(WatsonExceptionReport.BucketParamId.ExMethodName)));
			reportFile.WriteLine("P3(exceptionType)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonExceptionReport.BucketParamId>(WatsonExceptionReport.BucketParamId.ExceptionType)));
			reportFile.WriteLine("P4(callstackHash)={0}", base.BucketingParameter<WatsonExceptionReport.BucketParamId>(WatsonExceptionReport.BucketParamId.CallstackHash));
			WatsonExternalProcessReport.WriteReportFileCallStack(reportFile, this.callstack);
			WatsonExternalProcessReport.WriteReportFileDetailedExceptionInformation(reportFile, this.detailedExceptionInformation);
		}

		protected override void PrepareBucketingParameters()
		{
			base.PrepareBucketingParameters();
			base.SetBucketingParameter<WatsonExceptionReport.BucketParamId>(WatsonExceptionReport.BucketParamId.AssemblyVer, "unknown");
			base.SetBucketingParameter<WatsonExceptionReport.BucketParamId>(WatsonExceptionReport.BucketParamId.AssemblyName, "unknown");
		}

		private static void WriteReportFileCallStack(TextWriter reportFile, string callStack)
		{
			if (!string.IsNullOrEmpty(callStack))
			{
				reportFile.WriteLine(reportFile.NewLine);
				reportFile.WriteLine("----------------------------------------------------");
				reportFile.WriteLine("-------------------- Call Stack --------------------");
				reportFile.WriteLine("----------------------------------------------------");
				reportFile.WriteLine(callStack);
			}
		}

		private static void WriteReportFileDetailedExceptionInformation(TextWriter reportFile, string detailedExceptionInformation)
		{
			if (!string.IsNullOrEmpty(detailedExceptionInformation))
			{
				reportFile.WriteLine(reportFile.NewLine);
				reportFile.WriteLine("----------------------------------------------------");
				reportFile.WriteLine("---------------- Detailed Information --------------");
				reportFile.WriteLine("----------------------------------------------------");
				reportFile.WriteLine(detailedExceptionInformation);
			}
		}

		protected override void WriteReportTypeSpecificSection(XmlWriter reportFile)
		{
			base.WriteReportTypeSpecificSection(reportFile);
			using (SafeXmlTag safeXmlTag = new SafeXmlTag(reportFile, "detailed-info"))
			{
				safeXmlTag.SetContent(this.detailedExceptionInformation);
			}
		}

		private readonly string callstack;

		private readonly string detailedExceptionInformation;
	}
}
