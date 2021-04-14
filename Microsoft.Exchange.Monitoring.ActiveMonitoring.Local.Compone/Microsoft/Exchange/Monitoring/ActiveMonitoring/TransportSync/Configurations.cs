using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync
{
	public class Configurations
	{
		public Configurations(Dictionary<string, string> attributes)
		{
			this.server = LocalServer.GetServer();
			Configurations.CopyConfigurations(attributes, this.settings);
		}

		public Server Server
		{
			get
			{
				return this.server;
			}
		}

		public bool DatabaseConsistencyEnabled
		{
			get
			{
				return this.IsWorkItemEnabled(Configurations.DatabaseConsistencyEnabledName);
			}
		}

		public TimeSpan DatabaseConsistencyRecurrenceInterval
		{
			get
			{
				TimeSpan result;
				if (this.settings.ContainsKey(Configurations.DatabaseConsistencyRecurrenceIntervalName) && TimeSpan.TryParse(this.settings[Configurations.DatabaseConsistencyRecurrenceIntervalName], out result))
				{
					return result;
				}
				return Configurations.DefaultDatabaseConsistencyRecurrenceInterval;
			}
		}

		public int DatabaseConsistencyMonitorThreshold
		{
			get
			{
				int result;
				if (!int.TryParse(this.settings[Configurations.DatabaseConsistencyMonitorThresholdName], out result))
				{
					result = 10;
				}
				return result;
			}
		}

		public bool DeltaSyncEndpointUnreachableMonitorAndResponderEnabled
		{
			get
			{
				return this.IsWorkItemEnabled(Configurations.DeltaSyncEndpointUnreachableMonitorAndResponderEnabledName);
			}
		}

		public bool DeltaSyncServiceEndpointsLoadFailedMonitorAndResponderEnabled
		{
			get
			{
				return this.IsWorkItemEnabled(Configurations.DeltaSyncServiceEndpointsLoadFailedMonitorAndResponderEnabledName);
			}
		}

		public bool DeltaSyncPartnerAuthenticationFailedMonitorAndResponderEnabled
		{
			get
			{
				return this.IsWorkItemEnabled(Configurations.DeltaSyncPartnerAuthenticationFailedMonitorAndResponderEnabledName);
			}
		}

		public bool ProcessCrashDetectionEnabled
		{
			get
			{
				return this.IsWorkItemEnabled(Configurations.ProcessCrashDetectionEnabledName);
			}
		}

		public TimeSpan ProcessCrashDetectionRecurrenceInterval
		{
			get
			{
				TimeSpan result;
				if (this.settings.ContainsKey(Configurations.ProcessCrashDetectionRecurrenceIntervalName) && TimeSpan.TryParse(this.settings[Configurations.ProcessCrashDetectionRecurrenceIntervalName], out result))
				{
					return result;
				}
				return Configurations.DefaultProcessCrashDetectionRecurrenceInterval;
			}
		}

		public int ProcessCrashDetectionMonitorThreshold
		{
			get
			{
				int result;
				if (!int.TryParse(this.settings[Configurations.ProcessCrashDetectionMonitorThresholdName], out result))
				{
					result = 10;
				}
				return result;
			}
		}

		public bool RegistryAccessDeniedMonitorAndResponderEnabled
		{
			get
			{
				return this.IsWorkItemEnabled(Configurations.RegistryAccessDeniedMonitorAndResponderEnabledName);
			}
		}

		public bool ServiceAvailabilityEnabled
		{
			get
			{
				return this.IsWorkItemEnabled(Configurations.ServiceAvailabilityEnabledName);
			}
		}

		public TimeSpan ServiceAvailabilityRecurrenceInterval
		{
			get
			{
				TimeSpan result;
				if (this.settings.ContainsKey(Configurations.ServiceAvailabilityRecurrenceIntervalName) && TimeSpan.TryParse(this.settings[Configurations.ServiceAvailabilityRecurrenceIntervalName], out result))
				{
					return result;
				}
				return Configurations.DefaultServiceAvailabilityRecurrenceInterval;
			}
		}

		public bool SubscriptionSlaMissedMonitorAndResponderEnabled
		{
			get
			{
				return this.IsWorkItemEnabled(Configurations.SubscriptionSlaMissedMonitorAndResponderEnabledName);
			}
		}

		public int SubscriptionSlaMissedMonitorThreshold
		{
			get
			{
				int result;
				if (!int.TryParse(this.settings[Configurations.SubscriptionSlaMissedMonitorThresholdName], out result))
				{
					result = 12;
				}
				return result;
			}
		}

		public int SubscriptionSlaMissedPerfCounterThreshold
		{
			get
			{
				int result;
				if (!int.TryParse(this.settings[Configurations.SubscriptionSlaMissedPerfCounterThresholdName], out result))
				{
					result = 3900;
				}
				return result;
			}
		}

		public string this[string name]
		{
			get
			{
				if (!this.settings.ContainsKey(name))
				{
					return null;
				}
				return this.settings[name];
			}
		}

		public static Configurations CreateFromWorkDefinition(WorkDefinition workDefinition)
		{
			return new Configurations(workDefinition.Attributes);
		}

		public void Add(string key, string value)
		{
			this.settings.Add(key, value);
		}

		private static void CopyConfigurations(Dictionary<string, string> source, Dictionary<string, string> target)
		{
			foreach (string key in source.Keys)
			{
				target[key] = source[key];
			}
		}

		private bool IsWorkItemEnabled(string workitemName)
		{
			bool flag;
			return this.settings.ContainsKey(workitemName) && bool.TryParse(this.settings[workitemName], out flag) && flag;
		}

		private const int DefaultDatabaseConsistencyMonitorThreshold = 10;

		private const int DefaultProcessCrashDetectionMonitorThreshold = 10;

		private const int DefaultSubscriptionSlaMissedMonitorThreshold = 12;

		private const int DefaultSubscriptionSlaMissedPerfCounterThreshold = 3900;

		public static readonly TimeSpan GetExchangeDiagnosticInfoTimeout = TimeSpan.FromMinutes(2.0);

		public static readonly string TransportSyncManagerProcessName = "Microsoft.Exchange.TransportSyncManagerSvc";

		public static readonly string TransportSyncManagerServiceName = "MSExchangeTransportSyncManagerSvc";

		private static readonly string DatabaseConsistencyEnabledName = "DatabaseConsistencyEnabled";

		private static readonly string DatabaseConsistencyRecurrenceIntervalName = "DatabaseConsistencyRecurrenceInterval";

		private static readonly string DatabaseConsistencyMonitorThresholdName = "DatabaseConsistencyMonitorThreshold";

		private static readonly string DeltaSyncEndpointUnreachableMonitorAndResponderEnabledName = "DeltaSyncEndpointUnreachableMonitorAndResponderEnabled";

		private static readonly string DeltaSyncServiceEndpointsLoadFailedMonitorAndResponderEnabledName = "DeltaSyncServiceEndpointsLoadFailedMonitorAndResponderEnabled";

		private static readonly string DeltaSyncPartnerAuthenticationFailedMonitorAndResponderEnabledName = "DeltaSyncPartnerAuthenticationFailedMonitorAndResponderEnabled";

		private static readonly string ProcessCrashDetectionEnabledName = "ProcessCrashDetectionEnabled";

		private static readonly string ProcessCrashDetectionRecurrenceIntervalName = "ProcessCrashDetectionRecurrenceInterval";

		private static readonly string ProcessCrashDetectionMonitorThresholdName = "ProcessCrashDetectionMonitorThreshold";

		private static readonly string RegistryAccessDeniedMonitorAndResponderEnabledName = "RegistryAccessDeniedMonitorAndResponderEnabled";

		private static readonly string ServiceAvailabilityEnabledName = "ServiceAvailabilityEnabled";

		private static readonly string ServiceAvailabilityRecurrenceIntervalName = "ServiceAvailabilityRecurrenceInterval";

		private static readonly string SubscriptionSlaMissedMonitorAndResponderEnabledName = "SubscriptionSlaMissedMonitorAndResponderEnabled";

		private static readonly string SubscriptionSlaMissedMonitorThresholdName = "SubscriptionSlaMissedMonitorThreshold";

		private static readonly string SubscriptionSlaMissedPerfCounterThresholdName = "SubscriptionSlaMissedPerfCounterThreshold";

		private static readonly TimeSpan DefaultDatabaseConsistencyRecurrenceInterval = TimeSpan.FromSeconds(360.0);

		private static readonly TimeSpan DefaultProcessCrashDetectionRecurrenceInterval = TimeSpan.FromSeconds(360.0);

		private static readonly TimeSpan DefaultServiceAvailabilityRecurrenceInterval = TimeSpan.FromSeconds(360.0);

		private Dictionary<string, string> settings = new Dictionary<string, string>();

		private readonly Server server;
	}
}
