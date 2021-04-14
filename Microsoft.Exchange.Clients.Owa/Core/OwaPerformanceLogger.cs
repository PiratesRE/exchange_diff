using System;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Diagnostics.LatencyDetection;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class OwaPerformanceLogger
	{
		internal static void TracePerformance(UserContext userContext)
		{
			OwaContext owaContext = OwaContext.Current;
			if (ETWTrace.ShouldTraceCasStop(owaContext.TraceRequestId))
			{
				string userContext2 = string.Empty;
				HttpRequest request = owaContext.HttpContext.Request;
				int totalBytes = request.TotalBytes;
				string pathAndQuery = request.Url.PathAndQuery;
				ExchangePrincipal exchangePrincipal = userContext.ExchangePrincipal;
				if (exchangePrincipal != null)
				{
					userContext2 = exchangePrincipal.MailboxInfo.DisplayName;
				}
				Trace.TraceCasStop(CasTraceEventType.Owa, owaContext.TraceRequestId, totalBytes, 0, owaContext.LocalHostName, userContext2, "OwaModule", pathAndQuery, string.Empty);
			}
		}

		internal static void LogPerformanceStatistics(UserContext userContext)
		{
			OwaContext owaContext = OwaContext.Current;
			if (userContext.IsClientSideDataCollectingEnabled)
			{
				string str = "&v=";
				owaContext.HttpContext.Response.AppendToLog(str + Globals.ApplicationVersion);
			}
			StringBuilder stringBuilder = new StringBuilder(OwaPerformanceLogger.EstimatedIISStringBuilderCapacity);
			long requestLatencyMilliseconds = owaContext.RequestLatencyMilliseconds;
			if (userContext.HasValidMailboxSession())
			{
				ExchangePrincipal exchangePrincipal = userContext.ExchangePrincipal;
				if (exchangePrincipal != null)
				{
					string serverFqdn = exchangePrincipal.MailboxInfo.Location.ServerFqdn;
					if (serverFqdn != null)
					{
						stringBuilder.Append("&mbx=").Append(serverFqdn);
					}
				}
			}
			stringBuilder.Append("&sessionId=").Append(userContext.Key.UserContextId);
			stringBuilder.Append("&prfltncy=").Append(requestLatencyMilliseconds);
			uint num = 0U;
			long num2 = 0L;
			uint num3 = 0U;
			long num4 = 0L;
			if (owaContext.RequestExecution == RequestExecution.Local)
			{
				TaskPerformanceData rpcData = owaContext.RpcData;
				if (rpcData != null && rpcData.End != PerformanceData.Zero)
				{
					PerformanceData difference = rpcData.Difference;
					num = difference.Count;
					num2 = (long)difference.Milliseconds;
					PerformanceData ewsRpcData = owaContext.EwsRpcData;
					if (ewsRpcData != PerformanceData.Zero)
					{
						num += ewsRpcData.Count;
						num2 += (long)ewsRpcData.Milliseconds;
					}
					OwaPerformanceLogger.rpcLogger.AppendIISLogsEntry(stringBuilder, num, num2);
				}
				TaskPerformanceData ldapData = owaContext.LdapData;
				if (ldapData != null)
				{
					PerformanceData difference2 = ldapData.Difference;
					num3 = difference2.Count;
					num4 = (long)difference2.Milliseconds;
					PerformanceData ewsLdapData = owaContext.EwsLdapData;
					if (ewsLdapData != PerformanceData.Zero)
					{
						num3 += ewsLdapData.Count;
						num4 += (long)ewsLdapData.Milliseconds;
					}
					OwaPerformanceLogger.ldapLogger.AppendIISLogsEntry(stringBuilder, num3, num4);
				}
				OwaPerformanceLogger.AppendLatencyHeaders(num2, num4, requestLatencyMilliseconds);
				OwaPerformanceLogger.availabilityLogger.AppendIISLogsEntry(stringBuilder, owaContext.AvailabilityQueryCount, owaContext.AvailabilityQueryLatency);
			}
			owaContext.HttpContext.Response.AppendToLog(stringBuilder.ToString());
			if (Globals.CollectPerRequestPerformanceStats)
			{
				StringBuilder stringBuilder2 = new StringBuilder(OwaPerformanceLogger.EstimatedBreadcrumbStringBuilderCapacity);
				stringBuilder2.Append("Total: ").Append(requestLatencyMilliseconds).Append(" ms");
				owaContext.OwaPerformanceData.TotalLatency = requestLatencyMilliseconds;
				owaContext.OwaPerformanceData.KilobytesAllocated = (long)((ulong)owaContext.MemoryData.Difference.Count);
				if (owaContext.HasTrustworthyRequestCpuLatency)
				{
					stringBuilder2.Append(" CPU: ");
					stringBuilder2.Append(owaContext.RequestCpuLatencyMilliseconds).Append(" ms");
				}
				if (owaContext.RequestExecution == RequestExecution.Local)
				{
					owaContext.OwaPerformanceData.RpcCount = num;
					owaContext.OwaPerformanceData.RpcLatency = (int)num2;
					owaContext.OwaPerformanceData.LdapCount = num3;
					owaContext.OwaPerformanceData.LdapLatency = (int)num4;
					OwaPerformanceLogger.rpcLogger.AppendBreadcrumbEntry(stringBuilder2, num, num2);
					OwaPerformanceLogger.ldapLogger.AppendBreadcrumbEntry(stringBuilder2, num3, num4);
					OwaPerformanceLogger.availabilityLogger.AppendBreadcrumbEntry(stringBuilder2, owaContext.AvailabilityQueryCount, owaContext.AvailabilityQueryLatency);
					if (!string.IsNullOrEmpty(owaContext.EwsPerformanceHeader))
					{
						owaContext.OwaPerformanceData.TraceOther(owaContext.EwsPerformanceHeader);
					}
				}
				owaContext.UserContext.PerformanceConsoleNotifier.UpdatePerformanceData(owaContext.OwaPerformanceData, true);
				userContext.LogBreadcrumb(stringBuilder2.ToString());
			}
		}

		private static void AppendLatencyHeaders(long rpcLatency, long ldapLatency, long requestLatency)
		{
			HttpContext httpContext = HttpContext.Current;
			if (httpContext == null || httpContext.Response == null || !UserAgentUtilities.IsMonitoringRequest(httpContext.Request.UserAgent))
			{
				return;
			}
			try
			{
				httpContext.Response.AppendHeader("X-DiagInfoRpcLatency", rpcLatency.ToString());
				httpContext.Response.AppendHeader("X-DiagInfoLdapLatency", ldapLatency.ToString());
				httpContext.Response.AppendHeader("X-DiagInfoIisLatency", requestLatency.ToString());
			}
			catch (HttpException arg)
			{
				ExTraceGlobals.PerformanceTracer.TraceDebug<HttpException>(0L, "Exception happened while trying to append latency headers. Exception will be ignored: {0}", arg);
			}
		}

		private const string MailboxServerName = "&mbx=";

		private const string UserContextName = "&sessionId=";

		private const string IISLatency = "&prfltncy=";

		private const string BreadcrumbLatency = "Total: ";

		private const string BreadcrumbCpuLatency = " CPU: ";

		private const string Msec = " ms";

		private const int LatencyStringLength = 4;

		private const int CountStringLength = 3;

		private const string IISRpcCount = "&prfrpccnt=";

		private const string IISRpcLatency = "&prfrpcltncy=";

		private const string BreadcrumbRPC = " RPC#: ";

		private const string IISLdapCount = "&prfldpcnt=";

		private const string IISLdapLatency = "&prfldpltncy=";

		private const string BreadcrumbLDAP = " LDAP#: ";

		private const string IISAvailabilityCount = "&prfavlcnt=";

		private const string IISAvailabilityLatency = "&prfavlltncy=";

		private const string BreadcrumbAvailability = " Avail#: ";

		internal const string ClientPerfCounters = "cpc";

		internal const string ClientActionList = "calist";

		internal const string PerfMarkers = "pfmk";

		private static readonly int EstimatedIISStringBuilderCapacity = "&prfltncy=".Length + "&prfrpccnt=".Length + "&prfrpcltncy=".Length + "&prfldpcnt=".Length + "&prfldpltncy=".Length + "&prfavlcnt=".Length + "&prfavlltncy=".Length + 16 + 9;

		private static readonly int EstimatedBreadcrumbStringBuilderCapacity = "Total: ".Length + " CPU: ".Length + 2 * " ms".Length + " RPC#: ".Length + " LDAP#: ".Length + " Avail#: ".Length + 3 * (" (".Length + " ms)".Length) + 20 + 9;

		private static readonly PerformanceLogger rpcLogger = new PerformanceLogger("&prfrpccnt=", "&prfrpcltncy=", " RPC#: ");

		private static readonly PerformanceLogger ldapLogger = new PerformanceLogger("&prfldpcnt=", "&prfldpltncy=", " LDAP#: ");

		private static readonly PerformanceLogger availabilityLogger = new PerformanceLogger("&prfavlcnt=", "&prfavlltncy=", " Avail#: ");
	}
}
