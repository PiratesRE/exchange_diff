using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MRSSettingsLog : MRSScheduledLog<MRSSettingsData>, ILogProcessable
	{
		public MRSSettingsLog() : base(new MRSSettingsLog.MRSSettingsLogSchema(), new SimpleObjectLogConfiguration("MRSSetting", "MRSSettingsLogEnabled", "MRSSettingsLogMaxDirSize", "MRSSettingsLogMaxFileSize"))
		{
		}

		protected override bool IsLogEnabled
		{
			get
			{
				return ConfigBase<MRSConfigSchema>.GetConfig<bool>("MRSSettingsLogEnabled");
			}
		}

		protected override TimeSpan ScheduledLoggingPeriod
		{
			get
			{
				return ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("MRSSettingsLoggingPeriod");
			}
		}

		public void ProcessLogs()
		{
			if (!this.LogIsNeeded())
			{
				return;
			}
			MRSSettingsLogCollection config = ConfigBase<MRSConfigSchema>.GetConfig<MRSSettingsLogCollection>("MRSSettingsLogList");
			if (config == null)
			{
				MRSSettingsLog.PublishPeriodicExceptionNotification(string.Format("The value for {0} setting is corrupt. Check and correct setting value", "MRSSettingsLogList"));
				return;
			}
			foreach (MRSSettingsLogCollection.MRSSettingsLogElement mrssettingsLogElement in config.SettingsLogCollection)
			{
				try
				{
					MRSSettingsData loggingStatsData;
					if (StringComparer.OrdinalIgnoreCase.Equals(mrssettingsLogElement.SettingName, "IsJobPickupEnabled"))
					{
						using (IEnumerator enumerator2 = Enum.GetValues(typeof(RequestWorkloadType)).GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								object obj = enumerator2.Current;
								RequestWorkloadType requestWorkloadType = (RequestWorkloadType)obj;
								if (requestWorkloadType != RequestWorkloadType.None)
								{
									SettingsContextBase settingsContextBase = new GenericSettingsContext("RequestWorkloadType", requestWorkloadType.ToString(), null);
									using (settingsContextBase.Activate())
									{
										bool config2 = ConfigBase<MRSConfigSchema>.GetConfig<bool>(mrssettingsLogElement.SettingName);
										loggingStatsData = new MRSSettingsData
										{
											Context = string.Format("{0}={1}", "RequestWorkloadType", requestWorkloadType.ToString()),
											SettingName = "IsJobPickupEnabled",
											SettingValue = Convert.ToInt32(config2).ToString()
										};
									}
									this.Write(loggingStatsData);
								}
							}
							continue;
						}
					}
					if (StringComparer.OrdinalIgnoreCase.Equals(mrssettingsLogElement.SettingName, "IgnoreHealthMonitor"))
					{
						List<ResourceKey> list = new List<ResourceKey>
						{
							ADResourceKey.Key,
							ProcessorResourceKey.Local
						};
						using (List<ResourceKey>.Enumerator enumerator3 = list.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								ResourceKey resourceKey = enumerator3.Current;
								SettingsContextBase settingsContextBase2 = new GenericSettingsContext("WlmHealthMonitor", resourceKey.ToString(), null);
								using (settingsContextBase2.Activate())
								{
									bool config3 = ConfigBase<MRSConfigSchema>.GetConfig<bool>(mrssettingsLogElement.SettingName);
									loggingStatsData = new MRSSettingsData
									{
										Context = string.Format("{0}={1}", "WlmHealthMonitor", resourceKey.ToString()),
										SettingName = "IgnoreHealthMonitor",
										SettingValue = Convert.ToInt32(config3).ToString()
									};
								}
								this.Write(loggingStatsData);
							}
							continue;
						}
					}
					ConfigurationProperty configurationProperty;
					if (!ConfigBase<MRSConfigSchema>.Schema.TryGetConfigurationProperty(mrssettingsLogElement.SettingName, out configurationProperty))
					{
						throw new MRSSettingsLog.BadConfigSettingException(string.Format("Can not find corresponding name of MRS config setting specified by string {0}. Check if the setting name and correct it if needed", mrssettingsLogElement.SettingName));
					}
					string settingValue = string.Empty;
					if (configurationProperty.Type == typeof(bool))
					{
						settingValue = ConfigBase<MRSConfigSchema>.GetConfig<bool>(configurationProperty.Name).ToString();
					}
					else if (configurationProperty.Type == typeof(int))
					{
						settingValue = ConfigBase<MRSConfigSchema>.GetConfig<int>(configurationProperty.Name).ToString();
					}
					else if (configurationProperty.Type == typeof(long))
					{
						settingValue = ConfigBase<MRSConfigSchema>.GetConfig<long>(configurationProperty.Name).ToString();
					}
					else if (configurationProperty.Type == typeof(string))
					{
						settingValue = ConfigBase<MRSConfigSchema>.GetConfig<string>(configurationProperty.Name);
					}
					else
					{
						if (!(configurationProperty.Type == typeof(TimeSpan)))
						{
							throw new MRSSettingsLog.BadConfigSettingException(string.Format("Type {0} of a provided setting {1} is not supported by logging functionality. Check and correct list of the settings to be logged", configurationProperty.Type, mrssettingsLogElement.SettingName));
						}
						settingValue = ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>(configurationProperty.Name).ToString();
					}
					loggingStatsData = new MRSSettingsData
					{
						Context = "Server",
						SettingName = configurationProperty.Name,
						SettingValue = settingValue
					};
					this.Write(loggingStatsData);
				}
				catch (MRSSettingsLog.BadConfigSettingException ex)
				{
					MRSSettingsLog.PublishPeriodicExceptionNotification(ex.Message);
				}
			}
		}

		private static void PublishPeriodicExceptionNotification(string message)
		{
			EventNotificationItem.PublishPeriodic(ExchangeComponent.MailboxMigration.Name, ExchangeComponent.MailboxMigration.Name, "MRSConfigSettingsCorrupted", message, "MRSSettingsLogExceptionNotification", MRSSettingsLog.PeriodicExceptionNotificationPeriod, ResultSeverityLevel.Error, false);
		}

		public void Write(MRSSettingsData loggingStatsData)
		{
			base.LogObject(loggingStatsData);
		}

		private const string ContextPairOutputFormat = "{0}={1}";

		private const string ServerContextName = "Server";

		private const string PeriodicExceptionNotificationKey = "MRSSettingsLogExceptionNotification";

		private static readonly TimeSpan PeriodicExceptionNotificationPeriod = TimeSpan.FromHours(1.0);

		internal class MRSSettingsLogSchema : ObjectLogSchema
		{
			public override string Software
			{
				get
				{
					return "Microsoft Exchange Mailbox Replication Service";
				}
			}

			public override string LogType
			{
				get
				{
					return "MRSSettings Log";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<MRSSettingsData> OwnerResourceName = new ObjectLogSimplePropertyDefinition<MRSSettingsData>("Context", (MRSSettingsData d) => d.Context);

			public static readonly ObjectLogSimplePropertyDefinition<MRSSettingsData> OwnerResourceGuid = new ObjectLogSimplePropertyDefinition<MRSSettingsData>("SettingName", (MRSSettingsData d) => d.SettingName);

			public static readonly ObjectLogSimplePropertyDefinition<MRSSettingsData> OwnerResourceType = new ObjectLogSimplePropertyDefinition<MRSSettingsData>("SettingValue", (MRSSettingsData d) => d.SettingValue);
		}

		private class BadConfigSettingException : Exception
		{
			private BadConfigSettingException()
			{
			}

			public BadConfigSettingException(string exceptionMessage) : base(exceptionMessage)
			{
			}

			public BadConfigSettingException(string exceptionMessage, Exception innerException) : base(exceptionMessage, innerException)
			{
			}
		}
	}
}
