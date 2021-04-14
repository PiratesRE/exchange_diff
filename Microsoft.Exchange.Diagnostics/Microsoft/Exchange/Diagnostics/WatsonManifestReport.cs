using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Microsoft.Exchange.Diagnostics
{
	internal class WatsonManifestReport : WatsonReport
	{
		public WatsonManifestReport(string eventType, Process process) : base(eventType, process)
		{
		}

		protected override WatsonIssueType GetIssueTypeCode()
		{
			return WatsonIssueType.GenericReport;
		}

		protected override string GetIssueDetails()
		{
			return base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExceptionType);
		}

		protected override void WriteReportTypeSpecificSection(XmlWriter reportFile)
		{
		}

		protected override void WriteSpecializedPartOfTextReport(TextWriter reportFile)
		{
			reportFile.WriteLine("P1(flavor)={0}", base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.Flavor));
			reportFile.WriteLine("P2(appVersion)={0}", base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AppVersion));
			reportFile.WriteLine("P3(appName)={0}", base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AppName));
			reportFile.WriteLine("P4(assemblyName)={0}", base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AssemblyName));
			reportFile.WriteLine("P5(exMethodName)={0}", base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExMethodName));
			reportFile.WriteLine("P6(exceptionType)={0}", base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExceptionType));
			reportFile.WriteLine("P7(callstackHash)={0}", base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.CallstackHash));
			reportFile.WriteLine("P8(assemblyVer)={0}", base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AssemblyVer));
			reportFile.WriteLine();
		}
	}
}
