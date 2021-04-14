using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	public class RequestStatistics
	{
		public RequestStatistics()
		{
			this.statisticsDataPoints = new Dictionary<RequestStatistics.RequestStatItem, RequestStatistics.StatisticsDataPoint>();
			this.extendedStatisticsDataPoints = new Dictionary<string, string>();
		}

		public string RequestUrl { get; set; }

		public static RequestStatistics CreateRequestRequestStatistics(HttpContext httpContext)
		{
			RequestStatistics requestStatistics = null;
			if (httpContext.Items != null)
			{
				requestStatistics = (httpContext.Items[RequestStatistics.RequestStatsKey] as RequestStatistics);
				if (requestStatistics == null)
				{
					requestStatistics = new RequestStatistics();
					requestStatistics.RequestUrl = httpContext.Request.RawUrl;
					httpContext.Items[RequestStatistics.RequestStatsKey] = requestStatistics;
				}
			}
			return requestStatistics;
		}

		public void AddStatisticsDataPoint(RequestStatistics.RequestStatItem name, DateTime startTime, DateTime endTime)
		{
			if (this.statisticsDataPoints.ContainsKey(name))
			{
				this.statisticsDataPoints[name] = new RequestStatistics.StatisticsDataPoint(name, startTime, endTime);
				return;
			}
			this.statisticsDataPoints.Add(name, new RequestStatistics.StatisticsDataPoint(name, startTime, endTime));
		}

		public void AddExtendedStatisticsDataPoint(string name, string value)
		{
			if (this.extendedStatisticsDataPoints.ContainsKey(name))
			{
				this.extendedStatisticsDataPoints[name] = value;
				return;
			}
			this.extendedStatisticsDataPoints.Add(name, value);
		}

		public string GetStatisticsDataPointResult(RequestStatistics.RequestStatItem name)
		{
			if (this.statisticsDataPoints.ContainsKey(name))
			{
				RequestStatistics.StatisticsDataPoint statisticsDataPoint = this.statisticsDataPoints[name];
				return string.Format("{0}({1})", statisticsDataPoint.EndTime.Subtract(statisticsDataPoint.StartTime).TotalMilliseconds, statisticsDataPoint.StartTime.ToString("hh:mm:ss.ffftt"));
			}
			return string.Empty;
		}

		public string GetExtendedStatisticsDataPointResult()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in this.extendedStatisticsDataPoints.Keys)
			{
				stringBuilder.AppendFormat("{0}:{1};", text, this.extendedStatisticsDataPoints[text]);
			}
			return stringBuilder.ToString();
		}

		public static readonly string RequestStatsKey = "RequestStatistics";

		private Dictionary<RequestStatistics.RequestStatItem, RequestStatistics.StatisticsDataPoint> statisticsDataPoints;

		private Dictionary<string, string> extendedStatisticsDataPoints;

		public enum RequestStatItem
		{
			RequestResponseTime,
			HttpHandlerProcessRequestLatency,
			RbacPrincipalAcquireLatency,
			NewExchangeRunspaceConfigurationSettingsLatency,
			GetReportingSchemaLatency,
			NewRwsExchangeRunspaceConfigurationLatency,
			NewRbacPrincipalLatency,
			GetMetadataProviderLatency,
			GetQueryProviderLatency,
			CreateGenericTypeListForResults,
			CmdletResponseTime,
			InvokeCmdletLatency,
			InvokeCmdletExcludeRunspaceCreationLatency,
			InvokeCmdletExclusiveLatency,
			PowerShellCreateRunspaceLatency,
			ActivateReportingWebServiceHostLatency,
			DeactivateReportingWebServiceHostLatency,
			CreatePSHostLatency,
			InitializeRunspaceLatency,
			GetInitialSessionStateLatency,
			ConfigureRunspaceLatency,
			CreateRunspaceServerSettingsLatency
		}

		private struct StatisticsDataPoint
		{
			public StatisticsDataPoint(RequestStatistics.RequestStatItem name, DateTime startTime, DateTime endTime)
			{
				this.Name = name;
				this.StartTime = startTime;
				this.EndTime = endTime;
			}

			public RequestStatistics.RequestStatItem Name;

			public DateTime StartTime;

			public DateTime EndTime;
		}
	}
}
