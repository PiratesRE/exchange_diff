using System;
using System.Reflection;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess
{
	internal class PerfCounterEscalateResponder : EscalateResponder
	{
		static PerfCounterEscalateResponder()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 55, ".cctor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\Responders\\PerfCounterEscalateResponder.cs");
			PerfCounterEscalateResponder.LocalSite = topologyConfigurationSession.GetLocalSite().Name;
		}

		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string escalationTeam, string escalationSubjectUnhealthy, string escalationMessageUnhealthy, string perfCounterValueUnits, bool enabled = true, NotificationServiceClass notificationServiceClass = NotificationServiceClass.Urgent, int minimumSecondsBetweenEscalates = 14400, string dailySchedulePattern = "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59")
		{
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, serviceName, alertTypeId, alertMask, targetResource, targetHealthState, escalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, enabled, notificationServiceClass, minimumSecondsBetweenEscalates, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.AssemblyPath = PerfCounterEscalateResponder.AssemblyPath;
			responderDefinition.TypeName = PerfCounterEscalateResponder.TypeName;
			responderDefinition.Attributes[PerfCounterEscalateResponder.ValueUnitsKey] = perfCounterValueUnits;
			return responderDefinition;
		}

		internal override void GetEscalationSubjectAndMessage(MonitorResult monitorResult, out string escalationSubject, out string escalationMessage, bool rethrow = false, Action<ResponseMessageReader> textGeneratorModifier = null)
		{
			if (monitorResult.LastFailedProbeResultId > 0)
			{
				ProbeResult result = base.Broker.GetProbeResult(monitorResult.LastFailedProbeId, monitorResult.LastFailedProbeResultId).ExecuteAsync(base.LocalCancellationToken, base.TraceContext).Result;
				if (result != null)
				{
					string[] array = result.Error.Split(new char[]
					{
						'$'
					});
					if (array.Length == 3)
					{
						array = array[1].Split(new char[]
						{
							'|'
						});
						if (array.Length == 4)
						{
							string text = this.ReadAttribute(PerfCounterEscalateResponder.ValueUnitsKey, string.Empty);
							PerfCounterEscalateResponder.PassiveAlertData data = new PerfCounterEscalateResponder.PassiveAlertData
							{
								CounterName = array[0],
								Protocol = array[1],
								DestSite = array[2].ToUpper(),
								CounterValue = array[3] + text,
								LocalSite = PerfCounterEscalateResponder.LocalSite,
								FullText = result.Error
							};
							try
							{
								string diffProtcolsToDestSiteHtml;
								string sameProtocolToDiffSitesHtml;
								PerfCounterHelper.GetPerSiteCountersHtml(data.CounterName, data.Protocol, data.DestSite, text, out diffProtcolsToDestSiteHtml, out sameProtocolToDiffSitesHtml);
								data.DiffProtcolsToDestSiteHtml = diffProtcolsToDestSiteHtml;
								data.SameProtocolToDiffSitesHtml = sameProtocolToDiffSitesHtml;
								data.AdditionalCountersHtml = PerfCounterHelper.GetInterestingCountersHtml(data.Protocol);
							}
							catch (Exception ex)
							{
								PerfCounterEscalateResponder.PassiveAlertData data2 = data;
								data2.FullText = data2.FullText + "\n" + ex.ToString();
							}
							textGeneratorModifier = delegate(ResponseMessageReader reader)
							{
								reader.AddObjectResolver<PerfCounterEscalateResponder.PassiveAlertData>("PassiveAlertData", () => data);
							};
						}
					}
				}
			}
			base.GetEscalationSubjectAndMessage(monitorResult, out escalationSubject, out escalationMessage, rethrow, textGeneratorModifier);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(PerfCounterEscalateResponder).FullName;

		private static readonly string ValueUnitsKey = "ValueUnitsKey";

		private static readonly string LocalSite;

		internal class PassiveAlertData
		{
			public string CounterName = PerfCounterEscalateResponder.PassiveAlertData.NoData;

			public string Protocol = PerfCounterEscalateResponder.PassiveAlertData.NoData;

			public string DestSite = PerfCounterEscalateResponder.PassiveAlertData.NoData;

			public string CounterValue = PerfCounterEscalateResponder.PassiveAlertData.NoData;

			public string DiffProtcolsToDestSiteHtml = PerfCounterEscalateResponder.PassiveAlertData.NoData;

			public string SameProtocolToDiffSitesHtml = PerfCounterEscalateResponder.PassiveAlertData.NoData;

			public string AdditionalCountersHtml = PerfCounterEscalateResponder.PassiveAlertData.NoData;

			public string ProtocolLogHtml = PerfCounterEscalateResponder.PassiveAlertData.NoData;

			public string LocalSite = PerfCounterEscalateResponder.PassiveAlertData.NoData;

			public string FullText = PerfCounterEscalateResponder.PassiveAlertData.NoData;

			private static readonly string NoData = "No data.";
		}
	}
}
