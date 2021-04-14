using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace Microsoft.Exchange.Diagnostics
{
	internal class WatsonGenericReport : WatsonManifestReport
	{
		public WatsonGenericReport(string eventType, string appVersion, string appName, string assemblyVersion, string assemblyName, string exceptionType, string callstack, string callstackHash, string methodName, string detailedExceptionInformation) : base(eventType, null)
		{
			if (WatsonGenericReport.bucketingParamNames == null)
			{
				Interlocked.Exchange<string[]>(ref WatsonGenericReport.bucketingParamNames, new string[]
				{
					"flavor",
					"appVersion",
					"appName",
					"assemblyName",
					"exMethodName",
					"exceptionType",
					"callstackHash",
					"assemblyVer"
				});
			}
			WatsonGenericReport.CheckStringParam("appVersion", appVersion);
			WatsonGenericReport.CheckStringParam("appName", appName);
			WatsonGenericReport.CheckStringParam("assemblyVersion", assemblyVersion);
			WatsonGenericReport.CheckStringParam("assemblyName", assemblyName);
			WatsonGenericReport.CheckStringParam("exceptionType", exceptionType);
			WatsonGenericReport.CheckStringParam("callstackHash", callstackHash);
			WatsonGenericReport.CheckStringParam("methodName", methodName);
			this.appVersion = appVersion;
			this.appName = appName;
			this.assemblyVersion = assemblyVersion;
			this.assemblyName = assemblyName;
			this.exceptionType = exceptionType;
			this.callstack = callstack;
			this.callstackHash = callstackHash;
			this.methodName = methodName;
			this.detailedExceptionInformation = detailedExceptionInformation;
		}

		internal static string StringHashFromStackTrace(string stackTrace)
		{
			int num = 0;
			string result = "0";
			try
			{
				int num2 = 0;
				int num3 = 0;
				MatchCollection matchCollection = WatsonGenericReport.RegexFunctions.Matches(stackTrace);
				foreach (object obj in matchCollection)
				{
					Match match = (Match)obj;
					if (match.Groups.Count == 2)
					{
						string value = match.Groups[1].Value;
						if (num == 0)
						{
							num3 = WatsonReport.ComputeHash(value, num3);
						}
						if (!value.Contains("Microsoft"))
						{
							continue;
						}
						num++;
						num2 = WatsonReport.ComputeHash(value, num2);
					}
					if (num >= 10)
					{
						break;
					}
				}
				if (num == 0)
				{
					num2 = num3;
				}
				result = Convert.ToString(num2 & 65535, 16);
			}
			catch
			{
			}
			return result;
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
			return WatsonGenericReport.bucketingParamNames;
		}

		protected override void PrepareBucketingParameters()
		{
			base.PrepareBucketingParameters();
			try
			{
				this.appVersion = WatsonReport.ExchangeFormattedVersion(new Version(this.appVersion));
			}
			catch
			{
				this.appVersion = WatsonReport.ExchangeFormattedVersion(ExWatson.ApplicationVersion);
			}
			base.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AppVersion, this.appVersion);
			base.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AppName, this.appName);
			base.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AssemblyName, this.assemblyName);
			base.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExMethodName, this.methodName);
			base.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExceptionType, this.exceptionType);
			base.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.CallstackHash, this.callstackHash);
			base.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AssemblyVer, this.assemblyVersion);
		}

		protected override WatsonIssueType GetIssueTypeCode()
		{
			return WatsonIssueType.GenericReport;
		}

		protected override void WriteReportTypeSpecificSection(XmlWriter reportFile)
		{
			using (new SafeXmlTag(reportFile, "generic-report"))
			{
				using (SafeXmlTag safeXmlTag2 = new SafeXmlTag(reportFile, "callstack"))
				{
					safeXmlTag2.SetContent(this.callstack);
				}
				using (SafeXmlTag safeXmlTag3 = new SafeXmlTag(reportFile, "detailed-info"))
				{
					safeXmlTag3.SetContent(this.detailedExceptionInformation);
				}
			}
		}

		protected override void WriteSpecializedPartOfTextReport(TextWriter reportFile)
		{
			base.WriteReportFileHeader(reportFile, "Manifest Report");
			reportFile.WriteLine("P1(flavor)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.Flavor)));
			reportFile.WriteLine("P2(appVersion)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AppVersion)));
			reportFile.WriteLine("P3(appName)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AppName)));
			reportFile.WriteLine("P4(assemblyName)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AssemblyName)));
			reportFile.WriteLine("P5(exMethodName)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExMethodName)));
			reportFile.WriteLine("P6(exceptionType)={0}", WatsonReport.GetValidString(base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExceptionType)));
			reportFile.WriteLine("P7(callstackHash)={0}", base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.CallstackHash));
			reportFile.WriteLine("P8(assemblyVer)={0}", WatsonReport.GetValidString(this.assemblyVersion));
			WatsonGenericReport.WriteReportFileCallStack(reportFile, this.callstack);
			if (!string.IsNullOrEmpty(this.detailedExceptionInformation))
			{
				WatsonGenericReport.WriteReportFileDetailedExceptionInformation(reportFile, this.detailedExceptionInformation);
			}
		}

		protected override StringBuilder GetArchivedReportName()
		{
			string text = base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExMethodName);
			string text2 = base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AssemblyName);
			string text3 = base.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.CallstackHash);
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

		private static void CheckStringParam(string paramName, string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(paramName);
			}
			if (value.Length == 0)
			{
				throw new ArgumentException("An empty string is not an appropriate value here.", paramName);
			}
		}

		private static void WriteReportFileCallStack(TextWriter reportFile, string callStack)
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

		private static void WriteReportFileDetailedExceptionInformation(TextWriter reportFile, string detailedExceptionInformation)
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

		private static readonly Regex RegexFunctions = new Regex("\\s*at ([^\\(]*?)\\(.*?\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static string[] bucketingParamNames;

		private string appVersion;

		private string appName;

		private string assemblyVersion;

		private string assemblyName;

		private string exceptionType;

		private string callstack;

		private string callstackHash;

		private string methodName;

		private string detailedExceptionInformation;
	}
}
