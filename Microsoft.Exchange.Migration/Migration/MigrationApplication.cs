using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.SyncMigrationServicelet;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationApplication : IDiagnosable
	{
		public MigrationApplication(WaitHandle stopEvent, IProvisioningHandler provisioningHandler)
		{
			this.cache = new MigrationJobCache();
			this.waitHandles = new WaitHandle[]
			{
				stopEvent,
				this.cache.CacheUpdated
			};
			this.components = new MigrationComponent[]
			{
				new MigrationScanner("Scanner", stopEvent),
				new MigrationScheduler("Scheduler", stopEvent)
			};
			MigrationApplication.ProvisioningHandler = provisioningHandler;
		}

		internal static IProvisioningHandler ProvisioningHandler { get; private set; }

		public static bool IsMigrationTypeEnabled(MigrationType migrationType)
		{
			MigrationType migrationType2 = MigrationType.None;
			string config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<string>("SyncMigrationEnabledMigrationTypes");
			try
			{
				migrationType2 = (MigrationType)Enum.Parse(typeof(MigrationType), config);
			}
			catch (ArgumentException ex)
			{
				string diagnosticInfo = string.Format("Configuration value for '{0}' is not valid: '{1}'", "SyncMigrationEnabledMigrationTypes", config);
				MigrationApplication.NotifyOfError("IsMigrationTypeEnabled", ex, diagnosticInfo, new ExEventLog.EventTuple?(MSExchangeSyncMigrationEventLogConstants.Tuple_PermanentException));
			}
			return (migrationType2 & migrationType) == migrationType;
		}

		public void Process()
		{
			bool isMigrationServiceRpcEndpointEnabled = MigrationTestIntegration.Instance.IsMigrationServiceRpcEndpointEnabled;
			bool isMigrationNotificationRpcEndpointEnabled = MigrationTestIntegration.Instance.IsMigrationNotificationRpcEndpointEnabled;
			this.issueCache = new MigrationIssueCache(() => this.cache);
			try
			{
				ConfigBase<MigrationServiceConfigSchema>.InitializeConfigProvider(new Func<IConfigSchema, IConfigProvider>(ConfigProvider.CreateProvider));
				this.RegisterDiagnosticComponents();
				if (isMigrationServiceRpcEndpointEnabled)
				{
					MigrationServiceRpcSkeleton.StartServer(new MigrationServiceRpcImpl());
					MigrationLogger.Log(MigrationEventType.Information, "MigrationService RPC endpoint started.", new object[0]);
				}
				if (isMigrationNotificationRpcEndpointEnabled)
				{
					MigrationNotificationRpcSkeleton.StartServer(new MigrationNotificationRpcImpl(this.cache));
					MigrationLogger.Log(MigrationEventType.Information, "NotificationService RPC endpoint started.", new object[0]);
				}
				MigrationApplication.RegisterFaultInjectionHandler();
				this.issueCache.EnableScanning();
				ProvisioningCache.InitializeAppRegistrySettings("Powershell");
				TimeSpan config;
				do
				{
					config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationProcessorIdleRunDelay");
					foreach (MigrationComponent migrationComponent in this.components)
					{
						if (!migrationComponent.ShouldProcess() || this.waitHandles[0].WaitOne(0, false) || !ConfigBase<MigrationServiceConfigSchema>.GetConfig<bool>("SyncMigrationIsEnabled"))
						{
							MigrationLogger.Log(MigrationEventType.Verbose, "Skipping a pass of {0}", new object[]
							{
								migrationComponent.Name
							});
						}
						else
						{
							Stopwatch stopwatch = Stopwatch.StartNew();
							try
							{
								MigrationLogger.Log(MigrationEventType.Verbose, "Running a pass of {0}", new object[]
								{
									migrationComponent.Name
								});
								if (migrationComponent.Process(this.cache))
								{
									config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationProcessorActiveRunDelay");
									migrationComponent.DiagnosticInfo.LastWorkTime = ExDateTime.UtcNow;
								}
								migrationComponent.DiagnosticInfo.Duration = stopwatch.ElapsedMilliseconds;
								migrationComponent.DiagnosticInfo.LastRunTime = ExDateTime.UtcNow;
								MigrationLogger.Log(MigrationEventType.Verbose, "Component {0} pass completed, sec = {1}", new object[]
								{
									migrationComponent.Name,
									stopwatch.Elapsed.TotalSeconds
								});
							}
							catch (TransientException ex)
							{
								MigrationApplication.NotifyOfTransientException(ex, "MigrationApplication.Process: Component " + migrationComponent.Name);
							}
							catch (StoragePermanentException ex2)
							{
								MigrationApplication.NotifyOfPermanentException(ex2, "MigrationApplication.Process: Component " + migrationComponent.Name);
								break;
							}
							catch (MigrationPermanentException ex3)
							{
								MigrationApplication.NotifyOfPermanentException(ex3, "MigrationApplication.Process: Component " + migrationComponent.Name);
								break;
							}
						}
					}
					MigrationLogger.Log(MigrationEventType.Verbose, "MigrationApplication.Process: Sleeping between runs for {0} sec", new object[]
					{
						config.TotalSeconds
					});
				}
				while (WaitHandle.WaitAny(this.waitHandles, config, false) != 0);
			}
			finally
			{
				if (isMigrationServiceRpcEndpointEnabled)
				{
					MigrationServiceRpcSkeleton.StopServer();
				}
				if (isMigrationNotificationRpcEndpointEnabled)
				{
					MigrationNotificationRpcSkeleton.StopServer();
				}
				this.UnregisterDiagnosticComponents();
			}
		}

		public string GetDiagnosticComponentName()
		{
			return "MigrationApplication";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			XElement xelement2 = new XElement("MigrationComponentCollection", new XElement("count", this.components.Length));
			xelement.Add(xelement2);
			foreach (MigrationComponent migrationComponent in this.components)
			{
				xelement2.Add(migrationComponent.GetDiagnosticInfo(parameters));
			}
			return xelement;
		}

		internal static void NotifyOfCriticalError(Exception ex, string diagnosticInfo)
		{
			MigrationApplication.NotifyOfError("NotifyOfCriticalError", ex, diagnosticInfo, new ExEventLog.EventTuple?(MSExchangeSyncMigrationEventLogConstants.Tuple_CriticalError));
		}

		internal static void NotifyOfPermanentException(Exception ex, string diagnosticInfo)
		{
			MigrationApplication.NotifyOfError("NotifyOfPermanentException", ex, diagnosticInfo, new ExEventLog.EventTuple?(MSExchangeSyncMigrationEventLogConstants.Tuple_PermanentException));
		}

		internal static void NotifyOfIgnoredException(Exception ex, string diagnosticInfo)
		{
			MigrationApplication.NotifyOfError("NotifyOfIgnoredException", ex, diagnosticInfo, new ExEventLog.EventTuple?(MSExchangeSyncMigrationEventLogConstants.Tuple_IgnoredException));
		}

		internal static void NotifyOfTransientException(Exception ex, string diagnosticInfo)
		{
			MigrationApplication.NotifyOfError("NotifyOfTransientException", ex, diagnosticInfo, null);
		}

		internal static void NotifyOfCorruptJob(Exception ex, string diagnosticInfo)
		{
			MigrationApplication.NotifyOfError("NotifyOfCorruptJob", ex, diagnosticInfo, new ExEventLog.EventTuple?(MSExchangeSyncMigrationEventLogConstants.Tuple_CorruptMigrationJob));
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("Diagnostic Information: {0}", diagnosticInfo));
			stringBuilder.AppendLine(string.Format("Exception: {0}", ex));
			MigrationServiceFactory.Instance.PublishNotification(MigrationNotifications.CorruptJobNotification, stringBuilder.ToString());
		}

		internal static void NotifyOfCorruptJobItem(Exception ex, string diagnosticInfo)
		{
			MigrationApplication.NotifyOfError("NotifyOfCorruptJobItem", ex, diagnosticInfo, new ExEventLog.EventTuple?(MSExchangeSyncMigrationEventLogConstants.Tuple_CorruptMigrationJobItem));
			string text = ex.ToString();
			if (text.Contains("internal error:Remove-MergeRequest -Identity Microsoft.Exchange.Data.Storage.Management.MigrationPermanentException") || text.Contains("internal error:Remove-MoveRequest -Identity Microsoft.Exchange.Data.Storage.Management.MigrationPermanentException") || (text.Contains("Exception: Microsoft.Exchange.Data.Storage.Management.MigrationTransientException") && text.Contains("Microsoft.Exchange.Data.Directory.ADDataSession.IsTenantIdentity") && (text.Contains("RecipientTasks.RemoveMergeRequest.CleanupADEntry") || text.Contains("RecipientTasks.RemoveMoveRequest.CleanupADEntry"))))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("Diagnostic Information: {0}", diagnosticInfo));
			stringBuilder.AppendLine(string.Format("Exception: {0}", text));
			MigrationServiceFactory.Instance.PublishNotification(MigrationNotifications.CorruptJobItemNotification, stringBuilder.ToString());
		}

		internal static bool HasTransientErrorReachedThreshold<StatusType>(MigrationStatusData<StatusType> statusData) where StatusType : struct
		{
			MigrationUtil.ThrowOnNullArgument(statusData, "statusData");
			string settingName = "MigrationTransientErrorCountThreshold";
			string settingName2 = "MigrationTransientErrorIntervalThreshold";
			if (statusData != null && statusData.TransientErrorCount > ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>(settingName) && statusData.StateLastUpdated != null && statusData.StateLastUpdated.Value < ExDateTime.UtcNow - ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>(settingName2))
			{
				MigrationLogger.Log(MigrationEventType.Warning, "Transient error occurred too many times in a long time frame {0}", new object[]
				{
					statusData
				});
				return true;
			}
			MigrationLogger.Log(MigrationEventType.Verbose, "status data {0} with error count {1} and last update time {2} did not reach threshold count {3} timespan {4}", new object[]
			{
				statusData,
				statusData.TransientErrorCount,
				statusData.StateLastUpdated,
				ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>(settingName),
				ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>(settingName2)
			});
			return false;
		}

		internal static void RunOperationWithCulture(CultureInfo culture, Action operation)
		{
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
			try
			{
				Thread.CurrentThread.CurrentCulture = culture;
				Thread.CurrentThread.CurrentUICulture = culture;
				operation();
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
			}
		}

		internal static bool TryGetEnabledMigrationSession(IMigrationDataProvider dataProvider, bool initializeQueue, out MigrationSession session)
		{
			session = null;
			if (!dataProvider.ADProvider.IsMigrationEnabled)
			{
				MigrationLogger.Log(MigrationEventType.Warning, "TryGetMigrationJob: Skipping disabled migration for {0}", new object[]
				{
					dataProvider.MailboxName
				});
				return false;
			}
			session = MigrationSession.Get(dataProvider, initializeQueue);
			return true;
		}

		private static void NotifyOfError(string caller, Exception ex, string diagnosticInfo, ExEventLog.EventTuple? eventId)
		{
			MigrationUtil.ThrowOnNullArgument(ex, "ex");
			MigrationUtil.ThrowOnNullOrEmptyArgument(diagnosticInfo, "diagnosticInfo");
			string text = MigrationLogger.GetDiagnosticInfo(ex, diagnosticInfo);
			if (text.Length > 16384)
			{
				text = text.Substring(0, 16381) + "...";
			}
			MigrationEventType eventType = MigrationEventType.Warning;
			if (eventId != null)
			{
				string text2 = text + ": " + MigrationLogContext.Current.ToString();
				MigrationApplication.EventLogger.LogEvent(eventId.Value, string.Empty, new object[]
				{
					text2
				});
				eventType = MigrationEventType.Error;
			}
			MigrationLogger.Log(eventType, caller + ": " + text, new object[0]);
		}

		private static void RegisterFaultInjectionHandler()
		{
			string migrationFaultInjectionHandler = MigrationTestIntegration.Instance.MigrationFaultInjectionHandler;
			if (string.IsNullOrEmpty(migrationFaultInjectionHandler))
			{
				return;
			}
			string[] array = migrationFaultInjectionHandler.Split(new char[]
			{
				';'
			});
			if (array.Length != 2)
			{
				return;
			}
			string assemblyFile = array[0];
			string name = array[1];
			Assembly assembly = Assembly.LoadFrom(assemblyFile);
			Type type = assembly.GetType(name);
			IExceptionInjectionHandler exceptionInjectionHandler = (IExceptionInjectionHandler)Activator.CreateInstance(type);
			ExTraceGlobals.FaultInjectionTracer.RegisterExceptionInjectionCallback(exceptionInjectionHandler.Callback);
		}

		private void RegisterDiagnosticComponents()
		{
			ProcessAccessManager.RegisterComponent(this);
			ProcessAccessManager.RegisterComponent(this.cache);
			ProcessAccessManager.RegisterComponent(this.issueCache);
			ProcessAccessManager.RegisterComponent(ConfigBase<MigrationServiceConfigSchema>.Provider);
		}

		private void UnregisterDiagnosticComponents()
		{
			ProcessAccessManager.UnregisterComponent(ConfigBase<MigrationServiceConfigSchema>.Provider);
			ProcessAccessManager.UnregisterComponent(this.issueCache);
			ProcessAccessManager.UnregisterComponent(this.cache);
			ProcessAccessManager.UnregisterComponent(this);
		}

		private const int MaxCharsPerEventLog = 16384;

		private const int StopEventIndex = 0;

		private static readonly ExEventLog EventLogger = new ExEventLog(new Guid("99c3afcb-a5f9-45e1-a5f6-8a93f0144865"), "MSExchange Migration");

		private MigrationJobCache cache;

		private WaitHandle[] waitHandles;

		private MigrationComponent[] components;

		private MigrationIssueCache issueCache;
	}
}
