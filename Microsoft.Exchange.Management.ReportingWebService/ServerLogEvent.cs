using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal sealed class ServerLogEvent : ILogEvent
	{
		internal ServerLogEvent(string activityId, string cafeSeverName, RequestStatistics stat)
		{
			this.datapointProperties = new Dictionary<string, object>();
			this.datapointProperties.Add("ACTID", activityId);
			this.datapointProperties.Add("CAFE", cafeSeverName);
			this.datapointProperties.Add("URL", stat.RequestUrl);
			this.datapointProperties.Add("REQRESP", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.RequestResponseTime));
			this.datapointProperties.Add("REQPROC", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.HttpHandlerProcessRequestLatency));
			this.datapointProperties.Add("RPATIME", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.RbacPrincipalAcquireLatency));
			this.datapointProperties.Add("NERCS", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.NewExchangeRunspaceConfigurationSettingsLatency));
			this.datapointProperties.Add("GRS", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.GetReportingSchemaLatency));
			this.datapointProperties.Add("NRERC", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.NewRwsExchangeRunspaceConfigurationLatency));
			this.datapointProperties.Add("NRP", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.NewRbacPrincipalLatency));
			this.datapointProperties.Add("GMP", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.GetMetadataProviderLatency));
			this.datapointProperties.Add("GQP", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.GetQueryProviderLatency));
			this.datapointProperties.Add("CGTLFR", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.CreateGenericTypeListForResults));
			this.datapointProperties.Add("CMD", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.CmdletResponseTime));
			this.datapointProperties.Add("ICMD", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.InvokeCmdletLatency));
			this.datapointProperties.Add("ICMDERC", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.InvokeCmdletExcludeRunspaceCreationLatency));
			this.datapointProperties.Add("ICMDE", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.InvokeCmdletExclusiveLatency));
			this.datapointProperties.Add("PSCR", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.PowerShellCreateRunspaceLatency));
			this.datapointProperties.Add("ARWSH", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.ActivateReportingWebServiceHostLatency));
			this.datapointProperties.Add("DRWSH", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.DeactivateReportingWebServiceHostLatency));
			this.datapointProperties.Add("CPSH", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.CreatePSHostLatency));
			this.datapointProperties.Add("IR", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.InitializeRunspaceLatency));
			this.datapointProperties.Add("GISS", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.GetInitialSessionStateLatency));
			this.datapointProperties.Add("CRS", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.ConfigureRunspaceLatency));
			this.datapointProperties.Add("CRSS", stat.GetStatisticsDataPointResult(RequestStatistics.RequestStatItem.CreateRunspaceServerSettingsLatency));
			this.datapointProperties.Add("EX", stat.GetExtendedStatisticsDataPointResult());
		}

		public string EventId
		{
			get
			{
				return "GlobalActivity";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return this.datapointProperties;
		}

		private Dictionary<string, object> datapointProperties;
	}
}
