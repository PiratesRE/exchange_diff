using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Monitoring;
using Microsoft.Mapi;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class MailboxAssistantsProcessingEvents : AssistantTroubleshooterBase
	{
		public MailboxAssistantsProcessingEvents(PropertyBag fields) : base(fields)
		{
		}

		private uint MaxWaitTime
		{
			get
			{
				return (uint)(this.fields["MaxProcessingTimeInMinutes"] ?? 15U) * 60U;
			}
		}

		public override MonitoringData InternalRunCheck()
		{
			MonitoringData monitoringData = new MonitoringData();
			if (base.ExchangeServer.AdminDisplayVersion.Major < MailboxAssistantsProcessingEvents.minExpectedServerVersion.Major || (base.ExchangeServer.AdminDisplayVersion.Major == MailboxAssistantsProcessingEvents.minExpectedServerVersion.Major && base.ExchangeServer.AdminDisplayVersion.Minor < MailboxAssistantsProcessingEvents.minExpectedServerVersion.Minor))
			{
				monitoringData.Events.Add(new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5101, EventTypeEnumeration.Warning, Strings.TSMinServerVersion(MailboxAssistantsProcessingEvents.minExpectedServerVersion.ToString())));
			}
			else
			{
				MonitoringPerformanceCounter mdbstatusUpdateCounterValue = this.GetMDBStatusUpdateCounterValue();
				if (mdbstatusUpdateCounterValue != null)
				{
					monitoringData.PerformanceCounters.Add(mdbstatusUpdateCounterValue);
					if (mdbstatusUpdateCounterValue.Value > this.MaxWaitTime)
					{
						monitoringData.Events.Add(new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5206, EventTypeEnumeration.Error, Strings.AIDatabaseStatusPollThreadHung(base.ExchangeServer.Name, mdbstatusUpdateCounterValue.Value)));
					}
				}
				else
				{
					monitoringData.Events.Add(this.TSMDBperformanceCounterNotLoaded(base.ExchangeServer.Name, "Elapsed Time since Last Database Status Update Attempt"));
				}
				List<MdbStatus> onlineMDBList = base.GetOnlineMDBList();
				int maximumEventQueueSize = this.GetMaximumEventQueueSize(base.ExchangeServer.Fqdn);
				foreach (MdbStatus mdbStatus in onlineMDBList)
				{
					string mdbName = mdbStatus.MdbName;
					MonitoringPerformanceCounter mdblastEventPollingAttemptCounterValue = this.GetMDBLastEventPollingAttemptCounterValue(mdbName);
					if (mdblastEventPollingAttemptCounterValue == null)
					{
						monitoringData.Events.Add(this.TSMDBperformanceCounterNotLoaded(base.ExchangeServer.Name, "Elapsed Time since Last Event Polling Attempt"));
					}
					else
					{
						monitoringData.PerformanceCounters.Add(mdblastEventPollingAttemptCounterValue);
						if (mdblastEventPollingAttemptCounterValue.Value > this.MaxWaitTime)
						{
							monitoringData.Events.Add(new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5205, EventTypeEnumeration.Error, Strings.AIMDBLastEventPollingThreadHung(base.ExchangeServer.Name, mdbName, mdblastEventPollingAttemptCounterValue.Value)));
						}
					}
					MonitoringPerformanceCounter mdblastEventPolledCounterValue = this.GetMDBLastEventPolledCounterValue(mdbName);
					if (mdblastEventPolledCounterValue == null)
					{
						monitoringData.Events.Add(this.TSMDBperformanceCounterNotLoaded(base.ExchangeServer.Name, "Elapsed Time Since Last Event Polled"));
					}
					MonitoringPerformanceCounter mdbeventsInQueueCounterValue = this.GetMDBEventsInQueueCounterValue(mdbName);
					if (mdbeventsInQueueCounterValue == null)
					{
						monitoringData.Events.Add(this.TSMDBperformanceCounterNotLoaded(base.ExchangeServer.Name, "Events in queue"));
					}
					if (mdblastEventPolledCounterValue != null && mdbeventsInQueueCounterValue != null)
					{
						monitoringData.PerformanceCounters.Add(mdblastEventPolledCounterValue);
						monitoringData.PerformanceCounters.Add(mdbeventsInQueueCounterValue);
						if (mdbeventsInQueueCounterValue.Value >= (double)maximumEventQueueSize && mdblastEventPolledCounterValue.Value > this.MaxWaitTime)
						{
							monitoringData.Events.Add(new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5205, EventTypeEnumeration.Error, Strings.AIMDBLastEventPollingThreadHung(base.ExchangeServer.Name, mdbName, mdblastEventPolledCounterValue.Value)));
						}
					}
				}
			}
			return monitoringData;
		}

		private MonitoringPerformanceCounter GetMDBStatusUpdateCounterValue()
		{
			return this.GetMonitoringPerformanceCounter(base.ExchangeServer.Fqdn, "MsExchange Assistants - Per Database", "Elapsed Time since Last Database Status Update Attempt", "msexchangemailboxassistants-total");
		}

		private MonitoringPerformanceCounter GetMDBLastEventPollingAttemptCounterValue(string mdbName)
		{
			return this.GetMonitoringPerformanceCounter(base.ExchangeServer.Fqdn, "MsExchange Assistants - Per Database", "Elapsed Time since Last Event Polling Attempt", "msexchangemailboxassistants-" + mdbName);
		}

		private MonitoringPerformanceCounter GetMDBLastEventPolledCounterValue(string mdbName)
		{
			return this.GetMonitoringPerformanceCounter(base.ExchangeServer.Fqdn, "MsExchange Assistants - Per Database", "Elapsed Time Since Last Event Polled", "msexchangemailboxassistants-" + mdbName);
		}

		private MonitoringPerformanceCounter GetMDBEventsInQueueCounterValue(string mdbName)
		{
			return this.GetMonitoringPerformanceCounter(base.ExchangeServer.Fqdn, "MsExchange Assistants - Per Database", "Events in queue", "msexchangemailboxassistants-" + mdbName);
		}

		private int GetMaximumEventQueueSize(string machineName)
		{
			int result = 500;
			try
			{
				using (RegistryKey registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName))
				{
					if (registryKey != null)
					{
						using (RegistryKey registryKey2 = registryKey.OpenSubKey("System\\CurrentControlSet\\Services\\MSExchangeMailboxAssistants\\Parameters"))
						{
							if (registryKey2 != null)
							{
								result = (int)registryKey2.GetValue("MaximumEventQueueSize", 500);
							}
						}
					}
				}
			}
			catch (IOException)
			{
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			return result;
		}

		private MonitoringPerformanceCounter GetMonitoringPerformanceCounter(string machineName, string categoryName, string counterName, string instanceName)
		{
			MonitoringPerformanceCounter result = null;
			try
			{
				using (PerformanceCounter localizedPerformanceCounter = base.GetLocalizedPerformanceCounter(categoryName, counterName, instanceName, machineName))
				{
					float num = localizedPerformanceCounter.NextValue();
					if (num == 0f)
					{
						Thread.Sleep(1000);
						num = localizedPerformanceCounter.NextValue();
					}
					localizedPerformanceCounter.Close();
					result = new MonitoringPerformanceCounter(categoryName, counterName, instanceName, (double)num);
				}
			}
			catch (Win32Exception)
			{
			}
			catch (InvalidOperationException)
			{
			}
			return result;
		}

		private MonitoringEvent TSMDBperformanceCounterNotLoaded(string serverName, string counterName)
		{
			return new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5100, EventTypeEnumeration.Warning, Strings.TSMDBperformanceCounterNotLoaded(serverName, counterName));
		}

		public const string MaxProcessingTimeInMinutesPropertyName = "MaxProcessingTimeInMinutes";

		private const string MsExchangeAssistantPerDatabaseCategory = "MsExchange Assistants - Per Database";

		private const string MdbLastEventPollingAttemptCounter = "Elapsed Time since Last Event Polling Attempt";

		private const string MdbLastEventPolledCounter = "Elapsed Time Since Last Event Polled";

		private const string MdbEventsInQueueCounter = "Events in queue";

		private const string MdbStatusUpdateCounter = "Elapsed Time since Last Database Status Update Attempt";

		private const int DefaultMaximumEventQueueSize = 500;

		private const uint DefaultWaitTimeInMinutes = 15U;

		private const string AssistantParameterRegistryPath = "System\\CurrentControlSet\\Services\\MSExchangeMailboxAssistants\\Parameters";

		private const string MaximumEventQueueSizeRegistryName = "MaximumEventQueueSize";

		private static ServerVersion minExpectedServerVersion = new ServerVersion(14, 1, 0, 0);
	}
}
