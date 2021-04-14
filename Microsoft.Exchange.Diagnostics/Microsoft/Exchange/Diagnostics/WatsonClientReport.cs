using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Exchange.Diagnostics
{
	internal class WatsonClientReport : WatsonManifestReport
	{
		static WatsonClientReport()
		{
			WatsonClientReport.bucketingParamNames[0] = "serverFlavor";
			WatsonClientReport.bucketingParamNames[1] = "exVersion";
			WatsonClientReport.bucketingParamNames[2] = "appName";
			WatsonClientReport.bucketingParamNames[3] = "traceComponent";
			WatsonClientReport.bucketingParamNames[4] = "function";
			WatsonClientReport.bucketingParamNames[5] = "exceptionType";
			WatsonClientReport.bucketingParamNames[6] = "callstackHash";
			WatsonClientReport.bucketingParamNames[7] = "filename";
		}

		public WatsonClientReport(string traceComponent, string function, string exceptionMessage, string exceptionType, string originalCallStack, string callStack, int callStackHash, string fileName) : base("E12IE", null)
		{
			this.traceComponent = traceComponent;
			this.function = function;
			this.exceptionType = exceptionType;
			this.fileName = fileName;
			StringBuilder stringBuilder = new StringBuilder(exceptionType.Length + callStack.Length + 2);
			stringBuilder.AppendLine(exceptionType);
			stringBuilder.Append(callStack);
			this.reportCallStack = stringBuilder.ToString();
			this.callStackHash = callStackHash;
			StringBuilder stringBuilder2 = new StringBuilder(exceptionMessage.Length + exceptionType.Length + originalCallStack.Length + 4);
			stringBuilder2.AppendLine(exceptionMessage);
			stringBuilder2.AppendLine(exceptionType);
			stringBuilder2.Append(originalCallStack);
			this.detailedExceptionInformation = stringBuilder2.ToString();
		}

		public string DetailedExceptionInformation
		{
			get
			{
				return this.detailedExceptionInformation;
			}
		}

		internal string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		internal string ReportCallStack
		{
			get
			{
				return this.reportCallStack;
			}
		}

		internal static string[] BuildWatsonParameters(string flavor, string version, string traceComponent, string functionName, string exceptionType, string callstack, int callStackHash)
		{
			return new string[]
			{
				WatsonReport.GetValidString(flavor),
				WatsonReport.GetValidString(version),
				"OWAClient",
				WatsonReport.GetValidString(traceComponent),
				WatsonReport.GetValidString(functionName),
				WatsonReport.GetValidString(exceptionType),
				WatsonClientReport.GetStringHashFromString(callStackHash)
			};
		}

		internal string[] GetWatsonParameters()
		{
			this.PrepareBucketingParameters();
			return new string[]
			{
				base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.Flavor),
				base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.ExVersion),
				"OWAClient",
				base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.TraceComponent),
				base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.Function),
				base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.ExceptionType),
				base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.CallstackHash)
			};
		}

		protected override string GetShortParameter(uint bucketParamId, string longParameter)
		{
			if (bucketParamId == 4U && Uri.IsWellFormedUriString(longParameter, UriKind.RelativeOrAbsolute))
			{
				return longParameter.Trim();
			}
			return WatsonReport.GetShortParameter(longParameter.Trim());
		}

		protected override string[] GetBucketingParamNames()
		{
			return WatsonClientReport.bucketingParamNames;
		}

		protected override void PrepareBucketingParameters()
		{
			if (this.bucketingParametersPrepared)
			{
				return;
			}
			base.PrepareBucketingParameters();
			base.SetBucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.ExVersion, this.version);
			base.SetBucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.AppName, "OWAClient");
			base.SetBucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.TraceComponent, this.traceComponent);
			base.SetBucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.Function, this.function);
			base.SetBucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.ExceptionType, this.exceptionType);
			base.SetBucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.CallstackHash, WatsonClientReport.GetStringHashFromString(this.callStackHash));
			base.SetBucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.FileName, string.Empty);
			this.bucketingParametersPrepared = true;
		}

		protected override WatsonIssueType GetIssueTypeCode()
		{
			return WatsonIssueType.ScriptError;
		}

		protected override void WriteReportTypeSpecificSection(XmlWriter reportFile)
		{
			using (new SafeXmlTag(reportFile, "client-report"))
			{
				using (SafeXmlTag safeXmlTag2 = new SafeXmlTag(reportFile, "callstack"))
				{
					safeXmlTag2.SetContent(this.ReportCallStack);
				}
				using (SafeXmlTag safeXmlTag3 = new SafeXmlTag(reportFile, "detailed-info"))
				{
					safeXmlTag3.SetContent(this.DetailedExceptionInformation);
				}
			}
		}

		protected override void WriteSpecializedPartOfTextReport(TextWriter reportFile)
		{
			base.WriteReportFileHeader(reportFile, "Manifest Report: Non Fatal Error for OWA");
			reportFile.WriteLine("P1(flavor)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.Flavor)));
			reportFile.WriteLine("P2(exVersion)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.ExVersion)));
			reportFile.WriteLine("P3(appName)={0}", "OWAClient");
			reportFile.WriteLine("P4(traceComponent)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.TraceComponent)));
			reportFile.WriteLine("P5(function)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.Function)));
			reportFile.WriteLine("P6(exceptionType)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.ExceptionType)));
			reportFile.WriteLine("P7(callstackHash)={0}", base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.CallstackHash));
			reportFile.WriteLine("filename={0}", WatsonReport.GetValidString(this.fileName));
			WatsonClientReport.WriteReportFileClientCallStack(reportFile, this.ReportCallStack);
			WatsonClientReport.WriteReportFileClientDetailedExceptionInformation(reportFile, this.DetailedExceptionInformation);
		}

		protected override StringBuilder GetArchivedReportName()
		{
			string text = base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.Function);
			string text2 = base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.TraceComponent);
			string text3 = base.BucketingParameter<WatsonClientReport.BucketParamId>(WatsonClientReport.BucketParamId.CallstackHash);
			StringBuilder stringBuilder = new StringBuilder(text.Length + text2.Length + text3.Length + "ExWatsonReport.xml".Length + 3);
			stringBuilder.Append(text);
			stringBuilder.Append('-');
			stringBuilder.Append(text2);
			stringBuilder.Append('-');
			stringBuilder.Append(text3);
			stringBuilder.Append('-');
			stringBuilder.Append("ExWatsonReport.xml");
			return stringBuilder;
		}

		private static string GetStringHashFromString(int hash)
		{
			return Convert.ToString(hash & 65535, 16);
		}

		private static void WriteReportFileClientCallStack(TextWriter reportFile, string callStack)
		{
			if (callStack != null)
			{
				reportFile.WriteLine(reportFile.NewLine);
				reportFile.WriteLine("----------------------------------------------------");
				reportFile.WriteLine("-------------------- Call Stack --------------------");
				reportFile.WriteLine("----------------------------------------------------");
				reportFile.WriteLine(callStack);
			}
		}

		private static void WriteReportFileClientDetailedExceptionInformation(TextWriter reportFile, string detailedExceptionInformation)
		{
			if (detailedExceptionInformation != null)
			{
				reportFile.WriteLine(reportFile.NewLine);
				reportFile.WriteLine("----------------------------------------------------");
				reportFile.WriteLine("---------------- Detailed Information --------------");
				reportFile.WriteLine("----------------------------------------------------");
				reportFile.WriteLine(detailedExceptionInformation);
			}
		}

		private const string OwaClientAppName = "OWAClient";

		private static readonly string[] bucketingParamNames = new string[8];

		private readonly string version = WatsonReport.ExchangeFormattedVersion(ExWatson.ApplicationVersion);

		private readonly string fileName;

		private readonly string exceptionType;

		private readonly string reportCallStack;

		private readonly int callStackHash;

		private readonly string function;

		private readonly string traceComponent;

		private readonly string detailedExceptionInformation;

		private bool bucketingParametersPrepared;

		internal new enum BucketParamId
		{
			Flavor,
			ExVersion,
			AppName,
			TraceComponent,
			Function,
			ExceptionType,
			CallstackHash,
			FileName,
			_Count
		}
	}
}
