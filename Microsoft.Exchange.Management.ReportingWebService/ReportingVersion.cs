using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.ReportingWebService;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal class ReportingVersion
	{
		public static string LatestVersion
		{
			get
			{
				return ReportingVersion.V1;
			}
		}

		public static void EnableVersionZero()
		{
			if (!ReportingVersion.SupportedVersion.Contains(ReportingVersion.V0, StringComparer.OrdinalIgnoreCase))
			{
				ReportingVersion.SupportedVersion.Add(ReportingVersion.V0);
			}
		}

		public static string GetCurrentReportingVersion(HttpContext httpContext)
		{
			string text = (string)httpContext.Items["Exchange_RWS_Version"];
			if (string.IsNullOrEmpty(text))
			{
				bool flag = httpContext.Request.QueryString.AllKeys.Contains(ReportingVersion.QueryStringParameterName);
				bool flag2 = httpContext.Request.Headers.AllKeys.Contains(ReportingVersion.HttpHeaderName);
				if (flag && flag2)
				{
					ServiceDiagnostics.ThrowError(ReportingErrorCode.ErrorVersionAmbiguous, Strings.ErrorVersionAmbiguous);
				}
				else if (flag || flag2)
				{
					string version;
					if (flag)
					{
						version = httpContext.Request.QueryString[ReportingVersion.QueryStringParameterName];
						ExTraceGlobals.ReportingWebServiceTracer.TraceDebug<string>(0L, "[ReportingVersion::GetVersion] Version in query string: {0}", text);
					}
					else
					{
						version = httpContext.Request.Headers[ReportingVersion.HttpHeaderName];
						ExTraceGlobals.ReportingWebServiceTracer.TraceDebug<string>(0L, "[ReportingVersion::GetVersion] Version in header: {0}", text);
					}
					if (!ReportingVersion.IsVersionSupported(version, out text))
					{
						ServiceDiagnostics.ThrowError(ReportingErrorCode.ErrorInvalidVersion, Strings.ErrorInvalidVersion);
					}
				}
				else
				{
					ExTraceGlobals.ReportingWebServiceTracer.TraceDebug<string>(0L, "[ReportingVersion::GetVersion] Use the latest version: {0}", ReportingVersion.LatestVersion);
					text = ReportingVersion.LatestVersion;
				}
			}
			ExTraceGlobals.ReportingWebServiceTracer.Information<string>(0L, "[ReportingVersion::GetVersion] Version: {0}", text);
			httpContext.Items["Exchange_RWS_Version"] = text;
			return text;
		}

		public static void WriteVersionInfoInResponse(HttpContext httpContext)
		{
			string value = (string)httpContext.Items["Exchange_RWS_Version"];
			if (string.IsNullOrEmpty(value))
			{
				value = ReportingVersion.LatestVersion;
			}
			httpContext.Response.Headers[ReportingVersion.HttpHeaderName] = value;
		}

		private static bool IsVersionSupported(string version, out string supportedVersion)
		{
			version = version.Trim();
			bool flag = ReportingVersion.SupportedVersion.Any((string v) => v.Equals(version, StringComparison.OrdinalIgnoreCase));
			supportedVersion = (flag ? version.ToUpper() : null);
			return flag;
		}

		private const string HttpContextKey = "Exchange_RWS_Version";

		public static readonly string HttpHeaderName = "X-RWS-Version";

		public static readonly string QueryStringParameterName = "rws-version";

		public static readonly string V0 = "V0";

		public static readonly string V1 = "2013-V1";

		private static readonly List<string> SupportedVersion = new List<string>(new string[]
		{
			ReportingVersion.V1
		});
	}
}
