using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Diagnostics
{
	internal class WatsonHangReport : WatsonExceptionReport
	{
		public WatsonHangReport(string eventType, Process hungProcess, Exception exception) : base(eventType, hungProcess, exception, ReportOptions.ReportTerminateAfterSend | ReportOptions.DoNotFreezeThreads)
		{
		}

		protected override ProcSafeHandle GetProcessHandle()
		{
			if (base.IsProcessValid)
			{
				return base.GetProcessHandle();
			}
			return new ProcSafeHandle();
		}

		protected override void PrepareBucketingParameters()
		{
			string s = "unknown";
			if (base.Exception != null)
			{
				s = base.Exception.GetType().ToString();
			}
			base.SetBucketingParameter<WatsonExceptionReport.BucketParamId>(WatsonExceptionReport.BucketParamId.ExceptionType, WatsonReport.GetValidString(s));
			base.SetBucketingParameter<WatsonExceptionReport.BucketParamId>(WatsonExceptionReport.BucketParamId.AssemblyName, "unknown");
			base.SetBucketingParameter<WatsonExceptionReport.BucketParamId>(WatsonExceptionReport.BucketParamId.AssemblyVer, "unknown");
			base.SetBucketingParameter<WatsonExceptionReport.BucketParamId>(WatsonExceptionReport.BucketParamId.ExMethodName, "unknown");
			base.SetBucketingParameter<WatsonExceptionReport.BucketParamId>(WatsonExceptionReport.BucketParamId.CallstackHash, "0");
		}
	}
}
