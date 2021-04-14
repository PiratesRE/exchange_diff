using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.ServiceHost;
using Microsoft.Exchange.Servicelets.MigrationMonitor.MigrationServiceMonitor;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MigrationMonitor : Servicelet
	{
		public MigrationMonitor()
		{
			this.InitInstallAndLogPath();
			MigrationMonitor.SqlHelper = new MigMonSqlHelper();
			this.InitKnownStringIdMap();
		}

		internal static string ComputerName
		{
			get
			{
				return CommonUtils.LocalComputerName;
			}
		}

		internal static Dictionary<KnownStringType, Dictionary<string, int?>> KnownStringIdMap { get; set; }

		internal static MRSRequestCsvSchema MRSRequestCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.mrsRequestCsvSchemaInstance;
			}
		}

		internal static MRSFailureCsvSchema MRSFailureCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.mrsFailureCsvSchemaInstance;
			}
		}

		internal static MRSBadItemCsvSchema MRSBadItemCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.mrsBadItemCsvSchemaInstance;
			}
		}

		internal static MrsSessionStatisticsCsvSchema MrsSessionStatisticsCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.mrsSessionStatisticsCsvSchemaInstance;
			}
		}

		internal static MigrationServiceJobItemCsvSchema MigrationServiceJobItemCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.migrationServiceJobItemCsvSchemaInstance;
			}
		}

		internal static MigrationServiceJobCsvSchema MigrationServiceJobCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.migrationServiceJobCsvSchemaInstance;
			}
		}

		internal static MigrationServiceEndpointCsvSchema MigrationServiceEndpointCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.migrationServiceEndpointCsvSchemaInstance;
			}
		}

		internal static DatabaseInfoCsvSchema DatabaseInfoCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.databaseInfoCsvSchemaInstance;
			}
		}

		internal static MailboxStatsCsvSchema MailboxStatsCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.mailboxStatsCsvSchemaInstance;
			}
		}

		internal static MRSWorkloadAvailabilityCsvSchema MRSWorkloadAvailabilityCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.mrsWorkloadAvailabilityCsvSchemaInstance;
			}
		}

		internal static QueueMRSWorkStatsCsvSchema QueueMRSWorkStatsCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.queueMRSWorkStatsCsvSchemaInstance;
			}
		}

		internal static WLMResourceStatsCsvSchema WLMResourceStatsCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.wlmResoureStatsCsvSchemaInstance;
			}
		}

		internal static JobPickupResultsCsvSchema JobPickupResultsCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.jobPickupResultsCsvSchemaInstance;
			}
		}

		internal static DrumTestingResultCsvSchema DrumTestingResultCsvSchemaInstance
		{
			get
			{
				return MigrationMonitor.drumTestingResultCsvSchemaInstance;
			}
		}

		internal static AnchorContext MigrationMonitorContext { get; private set; } = new AnchorContext("MigrationMonitor", OrganizationCapability.TenantUpgrade, MigrationMonitor.CreateConfigSchema());

		internal static string ExchangeInstallPath { get; private set; }

		internal static string MigrationMonitorLogPath { get; private set; }

		internal static MigMonSqlHelper SqlHelper { get; private set; }

		public override void Work()
		{
			using (new ExchangeDiagnostics(MigrationMonitor.MigrationMonitorContext.Config))
			{
				TimeSpan config;
				do
				{
					if (this.ShouldRun())
					{
						config = MigrationMonitor.MigrationMonitorContext.Config.GetConfig<TimeSpan>("ActiveRunDelay");
						this.HandleKnownMigrationMonitorWorkExceptions(new Action(this.ProcessMRSLogs), "Processing MRS logs", new Action(MigrationMonitor.SqlHelper.ClearConnectionPool), "Clearing SQL connection pool");
						this.HandleKnownMigrationMonitorWorkExceptions(new Action(this.ProcessDCInfoLogs), "Processing DC information logs", new Action(MigrationMonitor.SqlHelper.ClearConnectionPool), "Clearing SQL connection pool");
					}
					else
					{
						config = MigrationMonitor.MigrationMonitorContext.Config.GetConfig<TimeSpan>("IdleRunDelay");
						MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "This server will not run, sleeping for {0}", new object[]
						{
							config
						});
					}
				}
				while (!base.StopEvent.WaitOne(config));
			}
		}

		internal static AnchorConfig CreateConfigSchema()
		{
			return new MigrationMonitor.MigrationMonitorConfig();
		}

		internal void SwitchConnectionStrings(MigMonSqlHelper.MigMonDatabaseSelection db = MigMonSqlHelper.MigMonDatabaseSelection.PrimaryMRSDatabase)
		{
			this.ResetCachedIds();
			MigrationMonitor.SqlHelper.GetConnectionStringFromConfig(db);
		}

		internal void ProcessMRSLogs()
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Starting to process MRS related logs.", new object[0]);
			List<BaseLogProcessor> list = new List<BaseLogProcessor>();
			MigrationMonitor.AddLogProcessorIfEnabled<MRSRequestLogProcessor>(list, "IsMRSLogProcessorEnabled");
			MigrationMonitor.AddLogProcessorIfEnabled<MRSFailureLogProcessor>(list, "IsMRSLogProcessorEnabled");
			MigrationMonitor.AddLogProcessorIfEnabled<MRSBadItemLogProcessor>(list, "IsMRSLogProcessorEnabled");
			MigrationMonitor.AddLogProcessorIfEnabled<MrsSessionStatisticsLogProcessor>(list, "IsMrsSessionStatisticsLogProcessorEnabled");
			MigrationMonitor.AddLogProcessorIfEnabled<MigrationServiceEndpointLogProcessor>(list, "IsMigServiceStatsLogProcessorEnabled");
			MigrationMonitor.AddLogProcessorIfEnabled<MigrationServiceJobLogProcessor>(list, "IsMigServiceStatsLogProcessorEnabled");
			MigrationMonitor.AddLogProcessorIfEnabled<MigrationServiceJobItemLogProcessor>(list, "IsMigServiceStatsLogProcessorEnabled");
			MigrationMonitor.AddLogProcessorIfEnabled<QueueMRSWorkStatsLogProcessor>(list, "IsQueueMRSWorkStatsLogProcessorEnabled");
			MigrationMonitor.AddLogProcessorIfEnabled<JobPickupResultsLogProcessor>(list, "IsJobPickupResultsLogProcessorEnabled");
			MigrationMonitor.AddLogProcessorIfEnabled<DrumTestingResultLogProcessor>(list, "IsDrumTestingResultLogProcessorEnabled");
			this.SwitchConnectionStrings(MigMonSqlHelper.MigMonDatabaseSelection.PrimaryMRSDatabase);
			this.RegisterServer();
			foreach (BaseLogProcessor baseLogProcessor in list)
			{
				baseLogProcessor.ProcessLogs();
			}
			this.PublishServerHealthStatus();
			this.UpdateHeartBeat();
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Finished processing MRS related logs.", new object[0]);
		}

		internal void ProcessDCInfoLogs()
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Starting to process DC info related logs.", new object[0]);
			List<BaseLogProcessor> list = new List<BaseLogProcessor>();
			MigrationMonitor.AddLogProcessorIfEnabled<DatabaseInfoLogProcessor>(list, "IsDatabaseInfoLogProcessorEnabled");
			MigrationMonitor.AddLogProcessorIfEnabled<MailboxStatsLogProcessor>(list, "IsMailboxStatsLogProcessorEnabled");
			MigrationMonitor.AddLogProcessorIfEnabled<MRSWorkloadAvailabilityLogProcessor>(list, "IsMRSWorkloadAvailabilityLogProcessorEnabled");
			this.SwitchConnectionStrings(MigMonSqlHelper.MigMonDatabaseSelection.DCInfoDatabase);
			this.RegisterServer();
			foreach (BaseLogProcessor baseLogProcessor in list)
			{
				baseLogProcessor.ProcessLogs();
			}
			this.UpdateHeartBeat();
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Finished processing DC info related logs.", new object[0]);
		}

		private static void AddLogProcessorIfEnabled<T>(List<BaseLogProcessor> logProcessorList, string configKeyName) where T : BaseLogProcessor, new()
		{
			if (!MigrationMonitor.MigrationMonitorContext.Config.GetConfig<bool>(configKeyName))
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "{0} is disabled by config.", new object[]
				{
					typeof(T).Name
				});
				return;
			}
			logProcessorList.Add(Activator.CreateInstance<T>());
		}

		private void HandleKnownMigrationMonitorWorkExceptions(Action action, string actionHint, Action postAction, string postActionHint)
		{
			try
			{
				action();
			}
			catch (SqlServerTimeoutException)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, "SQL server timed out. {0} :: skipping cycle.", new object[]
				{
					actionHint
				});
			}
			catch (LogFileReadException)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, "Error reading log file. {0} :: skipping cycle.", new object[]
				{
					actionHint
				});
			}
			catch (SqlServerUnreachableException exception)
			{
				this.ResetCachedIds();
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, exception, "{0} :: Unable to reach SQL Server. Skipping cycle.", new object[]
				{
					actionHint
				});
			}
			catch (LocalizedException exception2)
			{
				ExWatson.SendReport(exception2, ReportOptions.None, null);
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, exception2, "Error encountered during processing. {0} :: skipping cycle.", new object[]
				{
					actionHint
				});
			}
			finally
			{
				try
				{
					postAction();
				}
				catch (Exception exception3)
				{
					ExWatson.SendReport(exception3, ReportOptions.None, null);
					MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, exception3, "Error encountered during {0} post action execution", new object[]
					{
						postActionHint
					});
				}
			}
		}

		private void PublishServerHealthStatus()
		{
			if (DateTime.UtcNow <= this.nextServerHealthRefreshTime)
			{
				return;
			}
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Trying to update server health info.", new object[0]);
			try
			{
				MigMonHealthMonitor.PublishServerHealthStatus();
			}
			catch (HealthStatusPublishFailureException exception)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, exception, "Failed to publish server health info. Will try again next cycle.", new object[0]);
				return;
			}
			int num = int.MaxValue;
			try
			{
				num = Convert.ToInt32(MigrationMonitor.MigrationMonitorContext.Config.GetConfig<TimeSpan>("HealthStatusPublishInterval").TotalMinutes);
			}
			catch (OverflowException ex)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "avg interval too large, defaulting to intmax", new object[]
				{
					ex
				});
			}
			int num2 = num / 4;
			Random random = new Random();
			int num3 = random.Next(num - num2, num + num2);
			this.nextServerHealthRefreshTime = DateTime.UtcNow.AddMinutes((double)num3);
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Successfully published server health info. Next update at {0}", new object[]
			{
				this.nextServerHealthRefreshTime.ToString()
			});
		}

		private void InitInstallAndLogPath()
		{
			MigrationMonitor.ExchangeInstallPath = CommonUtils.GetExchangeInstallPath();
			MigrationMonitor.MigrationMonitorLogPath = Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\MigrationMonitorLogs");
		}

		private void UpdateHeartBeat()
		{
			MigrationMonitor.SqlHelper.SetHeartBeatTS();
		}

		private void RegisterServer()
		{
			string text = NativeHelpers.GetForestName();
			int num = text.IndexOf('.');
			if (num != -1)
			{
				text = text.Substring(0, num);
			}
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Registering Current Server: {0}   Forest: {1}     Site: {2}", new object[]
			{
				MigrationMonitor.ComputerName,
				text,
				NativeHelpers.GetSiteName(false)
			});
			MigrationMonitor.SqlHelper.RegisterLoggingServer(MigrationMonitor.ComputerName, text, NativeHelpers.GetSiteName(false), SysInfoHelper.GetCPUCores(true), SysInfoHelper.GetDiskSize(true));
		}

		private bool ShouldRun()
		{
			if (!MigrationMonitor.MigrationMonitorContext.Config.GetConfig<bool>("IsEnabled"))
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "MigrationMonitor is not enabled on this server", new object[0]);
				return false;
			}
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Verbose, "Migration Monitor is running on System.Environment.MachineName: {0}", new object[]
			{
				Environment.MachineName
			});
			return true;
		}

		private void InitKnownStringIdMap()
		{
			MigrationMonitor.KnownStringIdMap = new Dictionary<KnownStringType, Dictionary<string, int?>>();
			foreach (object obj in Enum.GetValues(typeof(KnownStringType)))
			{
				KnownStringType key = (KnownStringType)obj;
				MigrationMonitor.KnownStringIdMap.Add(key, new Dictionary<string, int?>());
			}
		}

		private void ResetCachedIds()
		{
			foreach (KnownStringType key in MigrationMonitor.KnownStringIdMap.Keys.ToArray<KnownStringType>())
			{
				MigrationMonitor.KnownStringIdMap[key] = new Dictionary<string, int?>();
			}
		}

		private const string KeyNameAvgHealthStatusPublishInterval = "HealthStatusPublishInterval";

		private const string AppName = "MigrationMonitor";

		private static MRSRequestCsvSchema mrsRequestCsvSchemaInstance = new MRSRequestCsvSchema();

		private static MRSFailureCsvSchema mrsFailureCsvSchemaInstance = new MRSFailureCsvSchema();

		private static MRSBadItemCsvSchema mrsBadItemCsvSchemaInstance = new MRSBadItemCsvSchema();

		private static MrsSessionStatisticsCsvSchema mrsSessionStatisticsCsvSchemaInstance = new MrsSessionStatisticsCsvSchema();

		private static MigrationServiceJobItemCsvSchema migrationServiceJobItemCsvSchemaInstance = new MigrationServiceJobItemCsvSchema();

		private static MigrationServiceJobCsvSchema migrationServiceJobCsvSchemaInstance = new MigrationServiceJobCsvSchema();

		private static MigrationServiceEndpointCsvSchema migrationServiceEndpointCsvSchemaInstance = new MigrationServiceEndpointCsvSchema();

		private static DatabaseInfoCsvSchema databaseInfoCsvSchemaInstance = new DatabaseInfoCsvSchema();

		private static MailboxStatsCsvSchema mailboxStatsCsvSchemaInstance = new MailboxStatsCsvSchema();

		private static MRSWorkloadAvailabilityCsvSchema mrsWorkloadAvailabilityCsvSchemaInstance = new MRSWorkloadAvailabilityCsvSchema();

		private static QueueMRSWorkStatsCsvSchema queueMRSWorkStatsCsvSchemaInstance = new QueueMRSWorkStatsCsvSchema();

		private static WLMResourceStatsCsvSchema wlmResoureStatsCsvSchemaInstance = new WLMResourceStatsCsvSchema();

		private static JobPickupResultsCsvSchema jobPickupResultsCsvSchemaInstance = new JobPickupResultsCsvSchema();

		private static DrumTestingResultCsvSchema drumTestingResultCsvSchemaInstance = new DrumTestingResultCsvSchema();

		private DateTime nextServerHealthRefreshTime;

		protected class MigrationMonitorConfig : AnchorConfig
		{
			internal MigrationMonitorConfig() : base("MigrationMonitor")
			{
				bool value = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Mrs.MigrationMonitor.Enabled;
				string value2 = "Server=tcp:l0tqt6mh64.database.windows.net,1433;Database=exo-mig-mon;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
				string value3 = "Server=tcp:caf5176sig.database.windows.net,1433;Database=exo-mig-mon-dcinfo;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
				if (CommonUtils.LocalComputerName.ToLower().Contains("eurprd"))
				{
					value2 = "Server=tcp:apu9k5pu1h.database.windows.net,1433;Database=exo-mig-mon-eur;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
				}
				if (CommonUtils.LocalComputerName.ToLower().Contains("apcprd"))
				{
					value2 = "Server=tcp:fbwrmjzyoo.database.windows.net,1433;Database=exo-mig-mon-apc;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
				}
				if (CommonUtils.LocalComputerName.ToLower().Contains("chnpr"))
				{
					value2 = "Server=tcp:odq4wwqizo.database.chinacloudapi.cn,1433;Database=exo-mig-mon-cn;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
					value3 = "Server=tcp:odq4wwqizo.database.chinacloudapi.cn,1433;Database=exo-mig-mon-dcinfo-cn;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
				}
				if (ExEnvironment.IsTestDomain)
				{
					value = false;
					value2 = "Server=tcp:wawqf20dco.database.windows.net,1433;Database=exo-mig-mon-test;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
					value3 = "Server=tcp:wawqf20dco.database.windows.net,1433;Database=exo-mig-mon-dcinfo-test;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
				}
				base.UpdateConfig<string>("ConnectionStringPrimary", value2);
				base.UpdateConfig<string>("ConnectionStringDCInfo", value3);
				base.UpdateConfig<bool>("IsEnabled", value);
				base.UpdateConfig<TimeSpan>("IdleRunDelay", TimeSpan.FromMinutes(15.0));
				base.UpdateConfig<TimeSpan>("ActiveRunDelay", TimeSpan.FromMinutes(15.0));
			}

			[ConfigurationProperty("HealthStatusPublishInterval", DefaultValue = "06:00:00")]
			public TimeSpan KeyNameAvgHealthStatusPublishInterval
			{
				get
				{
					return this.InternalGetConfig<TimeSpan>("KeyNameAvgHealthStatusPublishInterval");
				}
				set
				{
					this.InternalSetConfig<TimeSpan>(value, "KeyNameAvgHealthStatusPublishInterval");
				}
			}

			[ConfigurationProperty("SqlMaxRetryAttempts", DefaultValue = "5")]
			[IntegerValidator(MinValue = 0, MaxValue = 10000, ExcludeRange = false)]
			public int KeyNameSqlMaxRetryAttempts
			{
				get
				{
					return this.InternalGetConfig<int>("KeyNameSqlMaxRetryAttempts");
				}
				set
				{
					this.InternalSetConfig<int>(value, "KeyNameSqlMaxRetryAttempts");
				}
			}

			[ConfigurationProperty("SqlSleepBetweenRetryDuration", DefaultValue = "00:00:10")]
			[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "99999.00:00:00", ExcludeRange = false)]
			public TimeSpan KeyNameSqlSleepBetweenRetryDuration
			{
				get
				{
					return this.InternalGetConfig<TimeSpan>("KeyNameSqlSleepBetweenRetryDuration");
				}
				set
				{
					this.InternalSetConfig<TimeSpan>(value, "KeyNameSqlSleepBetweenRetryDuration");
				}
			}

			[IntegerValidator(MinValue = 0, MaxValue = 600, ExcludeRange = false)]
			[ConfigurationProperty("BulkInsertSqlCommandTimeout", DefaultValue = "30")]
			public int KeyNameBulkInsertSqlCommandTimeout
			{
				get
				{
					return this.InternalGetConfig<int>("KeyNameBulkInsertSqlCommandTimeout");
				}
				set
				{
					this.InternalSetConfig<int>(value, "KeyNameBulkInsertSqlCommandTimeout");
				}
			}

			[ConfigurationProperty("TransientErrorAlertTreshold", DefaultValue = "03:00:00")]
			[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "99999.00:00:00", ExcludeRange = false)]
			public TimeSpan KeyNameTransientErrorAlertTreshold
			{
				get
				{
					return this.InternalGetConfig<TimeSpan>("KeyNameTransientErrorAlertTreshold");
				}
				set
				{
					this.InternalSetConfig<TimeSpan>(value, "KeyNameTransientErrorAlertTreshold");
				}
			}

			[ConfigurationProperty("ConnectionStringPrimary", DefaultValue = "Server=tcp:l0tqt6mh64.database.windows.net,1433;Database=exo-mig-mon;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;")]
			public string KeyNameConnectionStringPrimary
			{
				get
				{
					return this.InternalGetConfig<string>("KeyNameConnectionStringPrimary");
				}
				set
				{
					this.InternalSetConfig<string>(value, "KeyNameConnectionStringPrimary");
				}
			}

			[ConfigurationProperty("ConnectionStringDCInfo", DefaultValue = "Server=tcp:caf5176sig.database.windows.net,1433;Database=exo-mig-mon-dcinfo;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;")]
			public string KeyNameConnectionStringDCInfo
			{
				get
				{
					return this.InternalGetConfig<string>("KeyNameConnectionStringDCInfo");
				}
				set
				{
					this.InternalSetConfig<string>(value, "KeyNameConnectionStringDCInfo");
				}
			}

			[ConfigurationProperty("MbxDBStatsFolder", DefaultValue = "Logging\\CompleteMailboxStats")]
			public string KeyNameMbxDBStatsFolder
			{
				get
				{
					return this.InternalGetConfig<string>("KeyNameMbxDBStatsFolder");
				}
				set
				{
					this.InternalSetConfig<string>(value, "KeyNameMbxDBStatsFolder");
				}
			}

			[ConfigurationProperty("DBStatsFileName", DefaultValue = "*DBStats*.log")]
			public string KeyNameDBStatsFileName
			{
				get
				{
					return this.InternalGetConfig<string>("KeyNameDBStatsFileName");
				}
				set
				{
					this.InternalSetConfig<string>(value, "KeyNameDBStatsFileName");
				}
			}

			[ConfigurationProperty("MbxStatsFileName", DefaultValue = "*MbxStats*.log")]
			public string KeyNameMbxStatsFileName
			{
				get
				{
					return this.InternalGetConfig<string>("KeyNameMbxStatsFileName");
				}
				set
				{
					this.InternalSetConfig<string>(value, "KeyNameMbxStatsFileName");
				}
			}

			[ConfigurationProperty("IsBaseLogProcessorEnabled", DefaultValue = false)]
			public bool KeyNameIsLogProcessorEnabled
			{
				get
				{
					return this.InternalGetConfig<bool>("KeyNameIsLogProcessorEnabled");
				}
				set
				{
					this.InternalSetConfig<bool>(value, "KeyNameIsLogProcessorEnabled");
				}
			}

			[ConfigurationProperty("IsMRSLogProcessorEnabled", DefaultValue = true)]
			public bool KeyNameIsMRSLogProcessorEnabled
			{
				get
				{
					return this.InternalGetConfig<bool>("KeyNameIsMRSLogProcessorEnabled");
				}
				set
				{
					this.InternalSetConfig<bool>(value, "KeyNameIsMRSLogProcessorEnabled");
				}
			}

			[ConfigurationProperty("IsQueueMRSWorkStatsLogProcessorEnabled", DefaultValue = true)]
			public bool KeyNameIsQueueStatsLogProcessorEnabled
			{
				get
				{
					return this.InternalGetConfig<bool>("KeyNameIsQueueStatsLogProcessorEnabled");
				}
				set
				{
					this.InternalSetConfig<bool>(value, "KeyNameIsQueueStatsLogProcessorEnabled");
				}
			}

			[ConfigurationProperty("IsJobPickupResultsLogProcessorEnabled", DefaultValue = true)]
			public bool KeyNameIsJobPickupResultsLogProcessorEnabled
			{
				get
				{
					return this.InternalGetConfig<bool>("KeyNameIsJobPickupResultsLogProcessorEnabled");
				}
				set
				{
					this.InternalSetConfig<bool>(value, "KeyNameIsJobPickupResultsLogProcessorEnabled");
				}
			}

			[ConfigurationProperty("IsWLMResourceStatsLogProcessorEnabled", DefaultValue = true)]
			public bool KeyNameIsWLMResourceStatsMRSLogProcessorEnabled
			{
				get
				{
					return this.InternalGetConfig<bool>("KeyNameIsWLMResourceStatsMRSLogProcessorEnabled");
				}
				set
				{
					this.InternalSetConfig<bool>(value, "KeyNameIsWLMResourceStatsMRSLogProcessorEnabled");
				}
			}

			[ConfigurationProperty("IsMigServiceStatsLogProcessorEnabled", DefaultValue = true)]
			public bool KeyNameIsMigServiceLogProcessorEnabled
			{
				get
				{
					return this.InternalGetConfig<bool>("KeyNameIsMigServiceLogProcessorEnabled");
				}
				set
				{
					this.InternalSetConfig<bool>(value, "KeyNameIsMigServiceLogProcessorEnabled");
				}
			}

			[ConfigurationProperty("IsDatabaseInfoLogProcessorEnabled", DefaultValue = true)]
			public bool KeyNameIsDatabaseInfoLogProcessorEnabled
			{
				get
				{
					return this.InternalGetConfig<bool>("KeyNameIsDatabaseInfoLogProcessorEnabled");
				}
				set
				{
					this.InternalSetConfig<bool>(value, "KeyNameIsDatabaseInfoLogProcessorEnabled");
				}
			}

			[ConfigurationProperty("IsMailboxStatsLogProcessorEnabled", DefaultValue = true)]
			public bool KeyNameIsMailboxStatsLogProcessorEnabled
			{
				get
				{
					return this.InternalGetConfig<bool>("KeyNameIsMailboxStatsLogProcessorEnabled");
				}
				set
				{
					this.InternalSetConfig<bool>(value, "KeyNameIsMailboxStatsLogProcessorEnabled");
				}
			}

			[ConfigurationProperty("IsMRSWorkloadAvailabilityLogProcessorEnabled", DefaultValue = true)]
			public bool KeyNameIsMRSWorkloadAvailabilityLogProcessorEnabled
			{
				get
				{
					return this.InternalGetConfig<bool>("KeyNameIsMRSWorkloadAvailabilityLogProcessorEnabled");
				}
				set
				{
					this.InternalSetConfig<bool>(value, "KeyNameIsMRSWorkloadAvailabilityLogProcessorEnabled");
				}
			}

			[ConfigurationProperty("IsMrsSessionStatisticsLogProcessorEnabled", DefaultValue = true)]
			public bool KeyNameIsMrsSessionStatisticsLogProcessorEnabled
			{
				get
				{
					return this.InternalGetConfig<bool>("KeyNameIsMrsSessionStatisticsLogProcessorEnabled");
				}
				set
				{
					this.InternalSetConfig<bool>(value, "KeyNameIsMrsSessionStatisticsLogProcessorEnabled");
				}
			}

			[ConfigurationProperty("IsDrumTestingResultLogProcessorEnabled", DefaultValue = false)]
			public bool KeyNameIsDrumTestingResultLogProcessorEnabled
			{
				get
				{
					return this.InternalGetConfig<bool>("KeyNameIsDrumTestingResultLogProcessorEnabled");
				}
				set
				{
					this.InternalSetConfig<bool>(value, "KeyNameIsDrumTestingResultLogProcessorEnabled");
				}
			}

			private const string ConnectionStringProdMRSNAM = "Server=tcp:l0tqt6mh64.database.windows.net,1433;Database=exo-mig-mon;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

			private const string ConnectionStringProdMRSEUR = "Server=tcp:apu9k5pu1h.database.windows.net,1433;Database=exo-mig-mon-eur;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

			private const string ConnectionStringProdMRSAPC = "Server=tcp:fbwrmjzyoo.database.windows.net,1433;Database=exo-mig-mon-apc;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

			private const string ConnectionStringProdMRSGALLATIN = "Server=tcp:odq4wwqizo.database.chinacloudapi.cn,1433;Database=exo-mig-mon-cn;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

			private const string ConnectionStringProdDCInfo = "Server=tcp:caf5176sig.database.windows.net,1433;Database=exo-mig-mon-dcinfo;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

			private const string ConnectionStringProdDCInfoGALLATIN = "Server=tcp:odq4wwqizo.database.chinacloudapi.cn,1433;Database=exo-mig-mon-dcinfo-cn;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

			private const string ConnectionStringTestMRS = "Server=tcp:wawqf20dco.database.windows.net,1433;Database=exo-mig-mon-test;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

			private const string ConnectionStringTestDCInfo = "Server=tcp:wawqf20dco.database.windows.net,1433;Database=exo-mig-mon-dcinfo-test;User ID=migmonsvclet;Password=UC2yX5d4sSMIMvpB4Vffww==;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
		}
	}
}
