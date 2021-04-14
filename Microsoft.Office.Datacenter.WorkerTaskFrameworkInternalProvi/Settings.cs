using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using Microsoft.Win32;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public static class Settings
	{
		static Settings()
		{
			Settings.uniformWorkload = Settings.workload;
			Settings.workload.Value = ((Settings.workload == "EXO") ? "Exchange" : Settings.workload);
		}

		public static string HttpProxyAvailabilityGroup
		{
			get
			{
				return Settings.httpProxyAvailabilityGroup;
			}
			set
			{
				Settings.httpProxyAvailabilityGroup.Value = value;
			}
		}

		public static string DefaultResultsLogPath
		{
			get
			{
				return Settings.defaultResultsLogPath;
			}
			set
			{
				Settings.defaultResultsLogPath.Value = value;
			}
		}

		public static string DefaultTraceLogPath
		{
			get
			{
				return Settings.defaultTraceLogPath;
			}
			set
			{
				Settings.defaultTraceLogPath.Value = value;
			}
		}

		public static bool IsResultsLoggingEnabled
		{
			get
			{
				return Settings.isResultsLoggingEnabled;
			}
			set
			{
				Settings.isResultsLoggingEnabled.Value = value;
			}
		}

		public static bool IsTraceLoggingEnabled
		{
			get
			{
				return Settings.isTraceLoggingEnabled;
			}
			set
			{
				Settings.isTraceLoggingEnabled.Value = value;
			}
		}

		public static int MaxLogAge
		{
			get
			{
				return Settings.maxLogAge;
			}
			set
			{
				Settings.maxLogAge.Value = value;
			}
		}

		public static int MaxResultsLogDirectorySizeInBytes
		{
			get
			{
				return Settings.maxResultsLogDirectorySizeInBytes;
			}
			set
			{
				Settings.maxResultsLogDirectorySizeInBytes.Value = value;
			}
		}

		public static int MaxResultsLogFileSizeInBytes
		{
			get
			{
				return Settings.maxResultsLogFileSizeInBytes;
			}
			set
			{
				Settings.maxResultsLogFileSizeInBytes.Value = value;
			}
		}

		public static long MaxTraceLogsDirectorySizeInBytes
		{
			get
			{
				return Settings.maxTraceLogsDirectorySizeInBytes;
			}
			set
			{
				Settings.maxTraceLogsDirectorySizeInBytes.Value = value;
			}
		}

		public static long MaxTraceLogFileSizeInBytes
		{
			get
			{
				return Settings.maxTraceLogFileSizeInBytes;
			}
			set
			{
				Settings.maxTraceLogFileSizeInBytes.Value = value;
			}
		}

		public static int MaxPersistentStateDirectorySizeInBytes
		{
			get
			{
				return Settings.maxPersistentStateDirectorySizeInBytes;
			}
			set
			{
				Settings.maxPersistentStateDirectorySizeInBytes.Value = value;
			}
		}

		public static int MaxPersistentStateFileSizeInBytes
		{
			get
			{
				return Settings.maxPersistentStateFileSizeInBytes;
			}
			set
			{
				Settings.maxPersistentStateFileSizeInBytes.Value = value;
			}
		}

		public static int ResultsLogBufferSizeInBytes
		{
			get
			{
				return Settings.resultsLogBufferSizeInBytes;
			}
			set
			{
				Settings.resultsLogBufferSizeInBytes.Value = value;
			}
		}

		public static int ResultsLogFlushIntervalInMinutes
		{
			get
			{
				return Settings.resultsLogFlushIntervalInMinutes;
			}
			set
			{
				Settings.resultsLogFlushIntervalInMinutes.Value = value;
			}
		}

		public static int TraceLogBufferSizeInBytes
		{
			get
			{
				return Settings.traceLogBufferSizeInBytes;
			}
			set
			{
				Settings.traceLogBufferSizeInBytes.Value = value;
			}
		}

		public static int TraceLogFlushIntervalInMinutes
		{
			get
			{
				return Settings.traceLogFlushIntervalInMinutes;
			}
			set
			{
				Settings.traceLogFlushIntervalInMinutes.Value = value;
			}
		}

		public static string OverrideRegistryPath
		{
			get
			{
				return Settings.registryPath;
			}
		}

		public static string SqlConnectionString
		{
			get
			{
				return Settings.connectionString;
			}
			set
			{
				Settings.connectionString.Value = value;
			}
		}

		public static string TopologyConnectionString
		{
			get
			{
				return Settings.topologyConnectionString;
			}
			set
			{
				Settings.topologyConnectionString.Value = value;
			}
		}

		public static string SmtpServerName
		{
			get
			{
				return Settings.smtpServerName;
			}
		}

		public static int SmtpServerPort
		{
			get
			{
				return Settings.smtpServerPort;
			}
		}

		public static string SenderMailAddress
		{
			get
			{
				return Settings.sendMailAddress;
			}
		}

		public static int ThrottleAmount
		{
			get
			{
				return Settings.throttleAmount;
			}
		}

		public static int WaitForWorkAmount
		{
			get
			{
				return Settings.waitForWorkAmount;
			}
		}

		public static int MaxWorkitemBatchSize
		{
			get
			{
				return Settings.maxWorkitemBatchSize;
			}
			set
			{
				Settings.maxWorkitemBatchSize.Value = value;
			}
		}

		public static int MaxRunningTasks
		{
			get
			{
				return Settings.maxRunningTasks;
			}
			set
			{
				Settings.maxRunningTasks.Value = value;
			}
		}

		public static int WaitAmountBeforeRestartRequest
		{
			get
			{
				return Settings.waitAmountBeforeRestartRequest;
			}
			set
			{
				Settings.waitAmountBeforeRestartRequest.Value = value;
			}
		}

		public static string HostedServiceName
		{
			get
			{
				return Settings.hostedServiceName;
			}
		}

		public static string InstanceName
		{
			get
			{
				return Settings.instanceName;
			}
			set
			{
				Settings.instanceName.Value = value;
			}
		}

		public static string DeploymentName
		{
			get
			{
				return Settings.deploymentName;
			}
		}

		public static int DeploymentId
		{
			get
			{
				return Settings.deploymentId;
			}
			set
			{
				Settings.deploymentId.Value = value;
			}
		}

		public static string Scope
		{
			get
			{
				return Settings.scope;
			}
			internal set
			{
				Settings.scope.Value = value;
			}
		}

		public static string UniformWorkload
		{
			get
			{
				return Settings.uniformWorkload;
			}
		}

		public static string Workload
		{
			get
			{
				return Settings.workload;
			}
			internal set
			{
				Settings.workload.Value = value;
			}
		}

		public static string WorkloadVersion
		{
			get
			{
				return Settings.workloadVersion;
			}
			internal set
			{
				Settings.workloadVersion.Value = value;
			}
		}

		public static string OverrideProbeExecutionLocation
		{
			get
			{
				return Settings.overrideProbeExecutionLocation;
			}
		}

		public static string Environment
		{
			get
			{
				return Settings.environment;
			}
			internal set
			{
				Settings.environment.Value = value;
			}
		}

		public static string EscalationEndpoint
		{
			get
			{
				return Settings.escalationEndpoint;
			}
		}

		public static string FileStorageLocation
		{
			get
			{
				return Settings.fileLocation;
			}
			set
			{
				Settings.fileLocation.Value = value;
			}
		}

		public static int WorkItemRetrievalDelay
		{
			get
			{
				return Settings.workItemRetrievalDelay;
			}
			set
			{
				Settings.workItemRetrievalDelay.Value = value;
			}
		}

		public static int? ServiceRestartDelay
		{
			get
			{
				return Settings.serviceRestartDelay;
			}
		}

		public static bool IsUseCurrentUserHiveForManagedAvailability
		{
			get
			{
				return Settings.isUseCurrentUserHiveForManagedAvailability;
			}
			set
			{
				Settings.isUseCurrentUserHiveForManagedAvailability.Value = value;
			}
		}

		public static RegistryKey ManagedAvailabilityServiceRegistryHive
		{
			get
			{
				if (Settings.IsUseCurrentUserHiveForManagedAvailability)
				{
					return Registry.CurrentUser;
				}
				return Registry.LocalMachine;
			}
		}

		public static string MachineName
		{
			get
			{
				return Settings.machineName;
			}
			set
			{
				Settings.machineName = value;
			}
		}

		public static int ResultHistoryWindowInMinutes
		{
			get
			{
				return Settings.resultHistoryWindowInMinutes;
			}
		}

		public static int ProbeResultHistoryWindowSize
		{
			get
			{
				return Settings.probeResultHistoryWindowSize;
			}
			internal set
			{
				Settings.probeResultHistoryWindowSize.Value = value;
			}
		}

		public static int MonitorResultHistoryWindowSize
		{
			get
			{
				return Settings.monitorResultHistoryWindowSize;
			}
		}

		public static int ResponderResultHistoryWindowSize
		{
			get
			{
				return Settings.responderResultHistoryWindowSize;
			}
		}

		public static int MaintenanceResultHistoryWindowSize
		{
			get
			{
				return Settings.maintenanceResultHistoryWindowSize;
			}
		}

		public static int QuarantineHours
		{
			get
			{
				return Settings.quarantineHours;
			}
		}

		public static int MaxPoisonCount
		{
			get
			{
				return Settings.maxPoisonCount;
			}
		}

		public static int NonRecurrentRetryIntervalSeconds
		{
			get
			{
				return Settings.nonRecurrentRetryIntervalSeconds;
			}
		}

		public static bool IsProductionEnvironment
		{
			get
			{
				return Settings.isProductionEnvironment;
			}
			internal set
			{
				Settings.isProductionEnvironment.Value = value;
			}
		}

		public static int BulkInsertBatchSize
		{
			get
			{
				return Settings.bulkInsertBatchSize;
			}
			set
			{
				Settings.bulkInsertBatchSize.Value = value;
			}
		}

		public static int ConsecutiveMaintenanceFailureThreshold
		{
			get
			{
				return Settings.consecutiveMaintenanceFailureThreshold;
			}
		}

		public static bool UseE14MonitoringTenant
		{
			get
			{
				return Settings.useE14MonitoringTenant;
			}
		}

		public static bool TracingCredentials
		{
			get
			{
				return Settings.tracingCredentials;
			}
		}

		public static int MaxObservers
		{
			get
			{
				return Settings.maxObservers;
			}
		}

		public static int MaxSubjects
		{
			get
			{
				return Settings.maxSubjects;
			}
		}

		public static int MaxRequestObservers
		{
			get
			{
				return Settings.maxRequestObservers;
			}
		}

		public static int MaxZombieSubjects
		{
			get
			{
				return Settings.maxZombieSubjects;
			}
		}

		public static int MaxMdbPerCasServer
		{
			get
			{
				return Settings.maxMdbPerCasServer;
			}
		}

		public static bool EnableStreamInsightPush
		{
			get
			{
				return Settings.enableStreamInsightPush;
			}
			set
			{
				Settings.enableStreamInsightPush.Value = value;
			}
		}

		public static string StreamInsightServerAddress
		{
			get
			{
				return Settings.streamInsightServerAddress;
			}
			set
			{
				Settings.streamInsightServerAddress.Value = value;
			}
		}

		public static string StreamInsightXamServerName
		{
			get
			{
				return Settings.streamInsightXamServerName;
			}
			set
			{
				Settings.streamInsightXamServerName.Value = value;
			}
		}

		public static string StreamInsightXamDatabaseName
		{
			get
			{
				return Settings.streamInsightXamDatabaseName;
			}
			set
			{
				Settings.streamInsightXamDatabaseName.Value = value;
			}
		}

		public static bool EnableOAuth
		{
			get
			{
				return Settings.enableOAuth;
			}
			set
			{
				Settings.enableOAuth.Value = value;
			}
		}

		public static string OAuthTenantName
		{
			get
			{
				return Settings.authTenantName;
			}
			set
			{
				Settings.authTenantName.Value = value;
			}
		}

		public static string OAuthAppId
		{
			get
			{
				return Settings.authAppId;
			}
			set
			{
				Settings.authAppId.Value = value;
			}
		}

		public static string OAuthSymmetricKey
		{
			get
			{
				return Settings.authSymmetricKey;
			}
			set
			{
				Settings.authSymmetricKey.Value = value;
			}
		}

		public static string OAuthCertificateName
		{
			get
			{
				return Settings.authCertificateName;
			}
			set
			{
				Settings.authCertificateName.Value = value;
			}
		}

		public static string KeynoteDataFeedBaseUrl
		{
			get
			{
				return Settings.keynoteDataFeedBaseUrl;
			}
			set
			{
				Settings.keynoteDataFeedBaseUrl.Value = value;
			}
		}

		public static string KeynoteDataPulseBaseUrl
		{
			get
			{
				return Settings.keynoteDataPulseBaseUrl;
			}
			set
			{
				Settings.keynoteDataPulseBaseUrl.Value = value;
			}
		}

		public static string KeynoteDataFeedAgreementIdUserMapping
		{
			get
			{
				return Settings.keynoteDataFeedAgreementIdUserMapping;
			}
			set
			{
				Settings.keynoteDataFeedAgreementIdUserMapping.Value = value;
			}
		}

		public static string KeynoteDataFeedBiz40AgreementIdUserMapping
		{
			get
			{
				return Settings.keynoteDataFeedBiz40AgreementIdUserMapping;
			}
			set
			{
				Settings.keynoteDataFeedBiz40AgreementIdUserMapping.Value = value;
			}
		}

		public static string KeynoteDataPulseAgreementIdUserMapping
		{
			get
			{
				return Settings.keynoteDataPulseAgreementIdUserMapping;
			}
			set
			{
				Settings.keynoteDataPulseAgreementIdUserMapping.Value = value;
			}
		}

		public static string KeynoteDataPulseBiz40AgreementIdUserMapping
		{
			get
			{
				return Settings.keynoteDataPulseBiz40AgreementIdUserMapping;
			}
			set
			{
				Settings.keynoteDataPulseBiz40AgreementIdUserMapping.Value = value;
			}
		}

		public static string KeynoteCredentials
		{
			get
			{
				return Settings.keynoteCredentials;
			}
			set
			{
				Settings.keynoteCredentials.Value = value;
			}
		}

		public static string RemotePowershellCertSubject
		{
			get
			{
				return Settings.remotePowershellCertSubject;
			}
			set
			{
				Settings.remotePowershellCertSubject.Value = value;
			}
		}

		public static bool RunningAsConsoleHost
		{
			get
			{
				return Settings.runningAsConsoleHost;
			}
			set
			{
				Settings.runningAsConsoleHost.Value = value;
			}
		}

		public static int NumberOfLastProbeResults
		{
			get
			{
				return Settings.numberOfLastProbeResults;
			}
			set
			{
				Settings.numberOfLastProbeResults.Value = value;
			}
		}

		public static int NumberOfLastMonitorResults
		{
			get
			{
				return Settings.numberOfLastMonitorResults;
			}
			set
			{
				Settings.numberOfLastMonitorResults.Value = value;
			}
		}

		public static int NumberOfLastResponderResults
		{
			get
			{
				return Settings.numberOfLastResponderResults;
			}
			set
			{
				Settings.numberOfLastResponderResults.Value = value;
			}
		}

		public static int NumberOfLastMaintenanceResults
		{
			get
			{
				return Settings.numberOfLastMaintenanceResults;
			}
			set
			{
				Settings.numberOfLastMaintenanceResults.Value = value;
			}
		}

		public static int BatchManagerBatchSize
		{
			get
			{
				return Settings.batchManagerBatchSize;
			}
			set
			{
				Settings.batchManagerBatchSize.Value = value;
			}
		}

		public static int BatchManagerBatchWaitTimeSeconds
		{
			get
			{
				return Settings.batchManagerBatchWaitTimeSeconds;
			}
			set
			{
				Settings.batchManagerBatchWaitTimeSeconds.Value = value;
			}
		}

		public static bool UseTransactionsInWorkItemGeneration
		{
			get
			{
				return Settings.useTransactionsInWorkItemGeneration;
			}
			set
			{
				Settings.useTransactionsInWorkItemGeneration.Value = value;
			}
		}

		public static bool IsPersistentStateEnabled
		{
			get
			{
				return Settings.isPersistentStateEnabled;
			}
			set
			{
				Settings.isPersistentStateEnabled.Value = value;
			}
		}

		public static bool IsCancelWorkItemsOnQuitRequestFeatureEnabled
		{
			get
			{
				return Settings.isCancelWorkItemsOnQuitRequestFeatureEnabled;
			}
			set
			{
				Settings.isCancelWorkItemsOnQuitRequestFeatureEnabled.Value = value;
			}
		}

		public static string ProbeSvcAddress
		{
			get
			{
				return Settings.probeSvcAddress;
			}
			set
			{
				Settings.probeSvcAddress.Value = value;
			}
		}

		public static string AccessControlHostName
		{
			get
			{
				return Settings.accessControlHostName;
			}
			set
			{
				Settings.accessControlHostName.Value = value;
			}
		}

		public static string AccessControlNamespace
		{
			get
			{
				return Settings.accessControlNamespace;
			}
			set
			{
				Settings.accessControlNamespace.Value = value;
			}
		}

		public static string AccessControlSigningCertificateFilePath
		{
			get
			{
				return Settings.accessControlSigningCertificateFilePath;
			}
			set
			{
				Settings.accessControlSigningCertificateFilePath.Value = value;
			}
		}

		public static string ProbeSvcClientPassword
		{
			get
			{
				return Settings.probeSvcClientPassword;
			}
			set
			{
				Settings.probeSvcClientPassword.Value = value;
			}
		}

		public static string ProbeSvcClientUsername
		{
			get
			{
				return Settings.probeSvcClientUsername;
			}
			set
			{
				Settings.probeSvcClientUsername.Value = value;
			}
		}

		public static string ProbeSvcCertificateFilePath
		{
			get
			{
				return Settings.probeSvcCertificateFilePath;
			}
			set
			{
				Settings.probeSvcCertificateFilePath.Value = value;
			}
		}

		public static string ProbeSvcCertificateDN
		{
			get
			{
				return Settings.probeSvcCertificateDN;
			}
			set
			{
				Settings.probeSvcCertificateDN.Value = value;
			}
		}

		public static bool UseSynchronousContinuationForWorkitemResults
		{
			get
			{
				return Settings.useSynchronousContinuationForWorkitemResults;
			}
			set
			{
				Settings.useSynchronousContinuationForWorkitemResults.Value = value;
			}
		}

		public static bool CalculateTimeoutFromBeginningOfExecution
		{
			get
			{
				return Settings.calculateTimeoutFromBeginningOfExecution;
			}
			set
			{
				Settings.calculateTimeoutFromBeginningOfExecution.Value = value;
			}
		}

		public static string StaticMonitoringPassword
		{
			get
			{
				return Settings.staticMonitoringPassword;
			}
			set
			{
				Settings.staticMonitoringPassword.Value = value;
			}
		}

		public static string SqlReadOnlyUser
		{
			get
			{
				return Settings.sqlReadOnlyUser;
			}
			set
			{
				Settings.sqlReadOnlyUser.Value = value;
			}
		}

		public static string SqlReadOnlyPassword
		{
			get
			{
				return Settings.sqlReadOnlyPassword;
			}
			set
			{
				Settings.sqlReadOnlyPassword.Value = value;
			}
		}

		public static string DataInsightSqlReadonlyUsername
		{
			get
			{
				return Settings.dataInsightSqlReadonlyUsername;
			}
			set
			{
				Settings.dataInsightSqlReadonlyUsername.Value = value;
			}
		}

		public static string DataInsightSqlReadonlyPassword
		{
			get
			{
				return Settings.dataInsightSqlReadonlyPassword;
			}
			set
			{
				Settings.dataInsightSqlReadonlyPassword.Value = value;
			}
		}

		public static string SqlReadWriteUser
		{
			get
			{
				return Settings.sqlReadWriteUser;
			}
			set
			{
				Settings.sqlReadWriteUser.Value = value;
			}
		}

		public static string SqlReadWritePassword
		{
			get
			{
				return Settings.sqlReadWritePassword;
			}
			set
			{
				Settings.sqlReadWritePassword.Value = value;
			}
		}

		public static string OpticsServer
		{
			get
			{
				return Settings.opticsServer;
			}
			set
			{
				Settings.opticsServer.Value = value;
			}
		}

		public static string SystemMonitoringInstance
		{
			get
			{
				return Settings.systemMonitoringInstance;
			}
			set
			{
				Settings.systemMonitoringInstance.Value = value;
			}
		}

		public static string OrcaVdir
		{
			get
			{
				return Settings.orcaVdir;
			}
			set
			{
				Settings.orcaVdir.Value = value;
			}
		}

		public static string ORCAUrlSuffix
		{
			get
			{
				return Settings.orcaUrlSuffix;
			}
			set
			{
				Settings.orcaUrlSuffix.Value = value;
			}
		}

		public static string ORCAATMUrl
		{
			get
			{
				return Settings.orcaATMUrl;
			}
			set
			{
				Settings.orcaATMUrl.Value = value;
			}
		}

		public static string OrcaClientCredential
		{
			get
			{
				return Settings.orcaClientCredential;
			}
			set
			{
				Settings.orcaClientCredential.Value = value;
			}
		}

		public static int CafeMailboxes
		{
			get
			{
				return Settings.cafeMailboxes;
			}
			set
			{
				if (value < 1)
				{
					Settings.cafeMailboxes.Value = 1;
					return;
				}
				if (value > Settings.MaxMdbPerCasServer)
				{
					Settings.cafeMailboxes.Value = Settings.MaxMdbPerCasServer;
					return;
				}
				Settings.cafeMailboxes.Value = value;
			}
		}

		public static int MaintenanceTimeoutWatsonHours
		{
			get
			{
				return Settings.maintenanceTimeoutWatsonHours;
			}
			set
			{
				Settings.maintenanceTimeoutWatsonHours.Value = value;
			}
		}

		public static bool IsCortex
		{
			get
			{
				return Settings.isCortex;
			}
			set
			{
				Settings.isCortex.Value = value;
			}
		}

		public static string AzureTableConnectionString
		{
			get
			{
				return Settings.azureTableConnectionString;
			}
			set
			{
				Settings.azureTableConnectionString.Value = value;
			}
		}

		public static int MaxNumberOfWorkUnits
		{
			get
			{
				return Settings.maxNumberOfWorkUnits;
			}
			set
			{
				Settings.maxNumberOfWorkUnits.Value = value;
			}
		}

		public static int MaxWorkUnitCost
		{
			get
			{
				return Settings.maxWorkUnitCost;
			}
			set
			{
				Settings.maxWorkUnitCost.Value = value;
			}
		}

		public static int MaxRecoveryAttempts
		{
			get
			{
				return Settings.maxRecoveryAttempts;
			}
			set
			{
				Settings.maxRecoveryAttempts.Value = value;
			}
		}

		public static int RecoveryThrottleTimeInSeconds
		{
			get
			{
				return Settings.recoveryThrottleTimeInSeconds;
			}
			set
			{
				Settings.recoveryThrottleTimeInSeconds.Value = value;
			}
		}

		public static int NumberOfHistoricalTables
		{
			get
			{
				return Settings.numberOfHistoricalTables;
			}
		}

		public static bool RestartOnPoisonedWorkItem
		{
			get
			{
				return Settings.restartOnPoisonedWorkItem;
			}
			set
			{
				Settings.restartOnPoisonedWorkItem.Value = value;
			}
		}

		public static string DataPartition
		{
			get
			{
				return Settings.dataPartition;
			}
			set
			{
				Settings.dataPartition.Value = value;
			}
		}

		public static string CortexDataPartitionRingKey
		{
			get
			{
				return Settings.cortexDataPartitionRingKey;
			}
			set
			{
				Settings.cortexDataPartitionRingKey.Value = value;
			}
		}

		public static string OrcaHubConnectionString
		{
			get
			{
				return Settings.orcaHubConnectionString;
			}
			set
			{
				Settings.orcaHubConnectionString.Value = value;
			}
		}

		public static string OrcaSpokeConnectionString
		{
			get
			{
				return Settings.orcaSpokeConnectionString;
			}
			set
			{
				Settings.orcaSpokeConnectionString.Value = value;
			}
		}

		public static string OrcaSpokeIdentity
		{
			get
			{
				return Settings.orcaSpokeIdentity;
			}
			set
			{
				Settings.orcaSpokeIdentity.Value = value;
			}
		}

		public static void ApplyOverride(string name, string value)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("name need be specified");
			}
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(Settings.registryPath))
			{
				registryKey.SetValue(name, value);
			}
		}

		public static void RemoveOverride(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("name need be specified");
			}
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Settings.registryPath, true))
			{
				registryKey.DeleteSubKey(name, false);
			}
		}

		public static void RemoveAllOverrides()
		{
			Registry.LocalMachine.DeleteSubKeyTree(Settings.registryPath, false);
		}

		internal static void RegisterTableMapping(Type tableType)
		{
			string tableName = Settings.GetTableName(tableType);
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			for (int i = 0; i < Settings.numberOfHistoricalTables; i++)
			{
				dictionary.Add(i, string.Format("{0}_{1}", tableName, i));
			}
			Settings.tableMapping.Add(tableType, dictionary);
		}

		internal static string GetTableName(Type type)
		{
			string result = Settings.GetTableMapping(type);
			int tableInstance = Settings.GetTableInstance();
			Dictionary<int, string> dictionary;
			if (Settings.tableMapping.TryGetValue(type, out dictionary))
			{
				dictionary.TryGetValue(tableInstance, out result);
			}
			return result;
		}

		internal static int GetTableInstance(int targetHour)
		{
			return targetHour / Settings.ProbeResultHistoryWindowSize % Settings.numberOfHistoricalTables;
		}

		internal static int GetTableInstance()
		{
			return Settings.GetTableInstance(DateTime.UtcNow.Hour);
		}

		internal static int GetNextTableInstance()
		{
			int tableInstance = Settings.GetTableInstance();
			return (tableInstance + 1) % Settings.numberOfHistoricalTables;
		}

		internal static string GetTableMapping(Type tableType)
		{
			TableAttribute tableAttribute = (TableAttribute)tableType.GetCustomAttributes(typeof(TableAttribute), false).Single<object>();
			return tableAttribute.Name ?? tableType.Name;
		}

		private static readonly string registryPath = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\WorkerTaskFramework\\Configuration", "v15");

		private static readonly Func<string, int> Int32Converter = (string s) => Convert.ToInt32(s);

		private static readonly Func<string, uint> UInt32Converter = (string s) => Convert.ToUInt32(s);

		private static readonly Func<string, long> Int64Converter = (string s) => Convert.ToInt64(s);

		private static readonly Func<string, bool> BooleanConverter = (string s) => Convert.ToBoolean(s);

		private static readonly Func<string, string> NoOpConverter = (string s) => s;

		private static int numberOfHistoricalTables = 3;

		private static Dictionary<Type, Dictionary<int, string>> tableMapping = new Dictionary<Type, Dictionary<int, string>>();

		private static OverridableSetting<string> fileLocation = new OverridableSetting<string>("FileStorageLocation", null, Settings.NoOpConverter, false);

		private static OverridableSetting<string> connectionString = new OverridableSetting<string>("SqlConnectionString", null, Settings.NoOpConverter, false);

		private static OverridableSetting<string> topologyConnectionString = new OverridableSetting<string>("TopologyConnectionString", null, Settings.NoOpConverter, false);

		private static OverridableSetting<bool> isUseCurrentUserHiveForManagedAvailability = new OverridableSetting<bool>("IsUseCurrentUserHiveForManagedAvailability", false, Settings.BooleanConverter, false);

		private static string machineName = System.Environment.MachineName;

		private static OverridableSetting<int> throttleAmount = new OverridableSetting<int>("ThrottleAmountInMilliSeconds", 50, Settings.Int32Converter, true);

		private static OverridableSetting<int> waitForWorkAmount = new OverridableSetting<int>("WaitForWorkAmountInMilliSeconds", 25, Settings.Int32Converter, true);

		private static OverridableSetting<int> maxWorkitemBatchSize = new OverridableSetting<int>("MaxWorkitemBatchSize", 5, Settings.Int32Converter, true);

		private static OverridableSetting<int> maxRunningTasks = new OverridableSetting<int>("MaxRunningTasks", 10, Settings.Int32Converter, true);

		private static OverridableSetting<int> waitAmountBeforeRestartRequest = new OverridableSetting<int>("WaitAmountBeforeRestartRequest", 10000, Settings.Int32Converter, true);

		private static OverridableSetting<int> workItemRetrievalDelay = new OverridableSetting<int>("WorkItemRetrievalDelay", 100, Settings.Int32Converter, true);

		private static OverridableSetting<int> resultHistoryWindowInMinutes = new OverridableSetting<int>("ResultHistoryWindowInMinutes", 60, Settings.Int32Converter, true);

		private static OverridableSetting<int> probeResultHistoryWindowSize = new OverridableSetting<int>("ProbeResultHistoryWindowSize", 30, Settings.Int32Converter, true);

		private static OverridableSetting<int> monitorResultHistoryWindowSize = new OverridableSetting<int>("MonitorResultHistoryWindowSize", 30, Settings.Int32Converter, true);

		private static OverridableSetting<int> responderResultHistoryWindowSize = new OverridableSetting<int>("ResponderResultHistoryWindowSize", 30, Settings.Int32Converter, true);

		private static OverridableSetting<int> maintenanceResultHistoryWindowSize = new OverridableSetting<int>("MaintenanceResultHistoryWindowSize", 30, Settings.Int32Converter, true);

		private static OverridableSetting<string> smtpServerName = new OverridableSetting<string>("SmtpServerName", null, Settings.NoOpConverter, true);

		private static OverridableSetting<int> smtpServerPort = new OverridableSetting<int>("Port", 25, Settings.Int32Converter, true);

		private static OverridableSetting<string> sendMailAddress = new OverridableSetting<string>("SenderMailAddress", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> instanceName = new OverridableSetting<string>("InstanceName", string.Empty, Settings.NoOpConverter, true);

		private static OverridableSetting<string> hostedServiceName = new OverridableSetting<string>("HostedServiceName", null, Settings.NoOpConverter, false);

		private static OverridableSetting<string> deploymentName = new OverridableSetting<string>("DeploymentName", null, Settings.NoOpConverter, false);

		private static OverridableSetting<int> deploymentId = new OverridableSetting<int>("DeploymentId", 0, Settings.Int32Converter, false);

		private static OverridableSetting<string> scope = new OverridableSetting<string>("Scope", null, Settings.NoOpConverter, false);

		private static OverridableSetting<string> workload = new OverridableSetting<string>("Workload", null, Settings.NoOpConverter, false);

		private static string uniformWorkload;

		private static OverridableSetting<string> workloadVersion = new OverridableSetting<string>("ExchangeVersion", null, Settings.NoOpConverter, false);

		private static OverridableSetting<string> environment = new OverridableSetting<string>("Environment", null, Settings.NoOpConverter, false);

		private static OverridableSetting<string> overrideProbeExecutionLocation = new OverridableSetting<string>("OverrideProbeExecutionLocation", null, Settings.NoOpConverter, false);

		private static OverridableSetting<string> escalationEndpoint = new OverridableSetting<string>("EscalationEndpoint", null, Settings.NoOpConverter, false);

		private static OverridableSetting<int> quarantineHours = new OverridableSetting<int>("QuarantineHours", 24, Settings.Int32Converter, true);

		private static OverridableSetting<int> maxPoisonCount = new OverridableSetting<int>("MaxPoisonCount", 5, Settings.Int32Converter, true);

		private static OverridableSetting<int> nonRecurrentRetryIntervalSeconds = new OverridableSetting<int>("NonRecurrentRetryIntervalSeconds", 5, Settings.Int32Converter, true);

		private static OverridableSetting<int> bulkInsertBatchSize = new OverridableSetting<int>("BulkInsertBatchSize", 500, Settings.Int32Converter, false);

		private static OverridableSetting<int> consecutiveMaintenanceFailureThreshold = new OverridableSetting<int>("ConsecutiveMaintenanceFailureThreshold", 3, Settings.Int32Converter, true);

		private static OverridableSetting<int?> serviceRestartDelay = new OverridableSetting<int?>("ServiceRestartDelay", null, (string s) => new int?(Convert.ToInt32(s)), true);

		private static OverridableSetting<bool> isProductionEnvironment = new OverridableSetting<bool>("IsProductionEnvironment", true, Settings.BooleanConverter, false);

		private static OverridableSetting<bool> tracingCredentials = new OverridableSetting<bool>("TracingCredentials", false, Settings.BooleanConverter, true);

		private static OverridableSetting<bool> useE14MonitoringTenant = new OverridableSetting<bool>("UseE14MonitoringTenant", false, Settings.BooleanConverter, true);

		private static OverridableSetting<int> maxObservers = new OverridableSetting<int>("MaxObservers", 3, Settings.Int32Converter, true);

		private static OverridableSetting<int> maxSubjects = new OverridableSetting<int>("MaxSubjects", 10, Settings.Int32Converter, true);

		private static OverridableSetting<int> maxRequestObservers = new OverridableSetting<int>("MaxRequestObservers", 10, Settings.Int32Converter, true);

		private static OverridableSetting<int> maxZombieSubjects = new OverridableSetting<int>("MaxZombieSubjects", 20, Settings.Int32Converter, true);

		private static OverridableSetting<int> maxMdbPerCasServer = new OverridableSetting<int>("MaxMdbPerCasServer", 200, Settings.Int32Converter, true);

		private static OverridableSetting<string> httpProxyAvailabilityGroup = new OverridableSetting<string>("HttpProxyAvailabilityGroup", null, Settings.NoOpConverter, true);

		private static OverridableSetting<bool> isResultsLoggingEnabled = new OverridableSetting<bool>("IsResultsLoggingEnabled", false, Settings.BooleanConverter, true);

		private static OverridableSetting<bool> isTraceLoggingEnabled = new OverridableSetting<bool>("IsTraceLoggingEnabled", false, Settings.BooleanConverter, true);

		private static OverridableSetting<string> defaultResultsLogPath = new OverridableSetting<string>("DefaultResultsLogPath", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> defaultTraceLogPath = new OverridableSetting<string>("DefaultTraceLogPath", null, Settings.NoOpConverter, true);

		private static OverridableSetting<int> maxLogAge = new OverridableSetting<int>("MaxLogAge", 7, Settings.Int32Converter, true);

		private static OverridableSetting<int> maxResultsLogDirectorySizeInBytes = new OverridableSetting<int>("MaxResultsLogDirectorySizeInBytes", 209715200, Settings.Int32Converter, true);

		private static OverridableSetting<int> maxResultsLogFileSizeInBytes = new OverridableSetting<int>("MaxResultsLogFileSizeInBytes", 104857600, Settings.Int32Converter, true);

		private static OverridableSetting<long> maxTraceLogsDirectorySizeInBytes = new OverridableSetting<long>("MaxTraceLogDirectorySizeInBytes", (long)((ulong)int.MinValue), Settings.Int64Converter, true);

		private static OverridableSetting<long> maxTraceLogFileSizeInBytes = new OverridableSetting<long>("MaxTraceLogFileSizeInBytes", 268435456L, Settings.Int64Converter, true);

		private static OverridableSetting<int> maxPersistentStateDirectorySizeInBytes = new OverridableSetting<int>("MaxPersistentStateDirectorySizeInBytes", 209715200, Settings.Int32Converter, true);

		private static OverridableSetting<int> maxPersistentStateFileSizeInBytes = new OverridableSetting<int>("MaxProbePersistentStateFileSizeInBytes", 52428800, Settings.Int32Converter, true);

		private static OverridableSetting<int> resultsLogBufferSizeInBytes = new OverridableSetting<int>("ResultsLogBufferSizeInBytes", 524288, Settings.Int32Converter, true);

		private static OverridableSetting<int> resultsLogFlushIntervalInMinutes = new OverridableSetting<int>("ResultsLogFlushIntervalInMinutes", 15, Settings.Int32Converter, true);

		private static OverridableSetting<int> traceLogBufferSizeInBytes = new OverridableSetting<int>("TraceLogBufferSizeInBytes", 1024, Settings.Int32Converter, true);

		private static OverridableSetting<int> traceLogFlushIntervalInMinutes = new OverridableSetting<int>("TraceLogFlushIntervalInMinutes", 1, Settings.Int32Converter, true);

		private static OverridableSetting<bool> enableStreamInsightPush = new OverridableSetting<bool>("EnableStreamInsightPush", false, Settings.BooleanConverter, true);

		private static OverridableSetting<string> streamInsightServerAddress = new OverridableSetting<string>("StreamInsightServerAddress", null, Settings.NoOpConverter, true);

		private static OverridableSetting<bool> enableOAuth = new OverridableSetting<bool>("EnableOAuth", false, Settings.BooleanConverter, true);

		private static OverridableSetting<string> authTenantName = new OverridableSetting<string>("OAuthTenantName", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> authAppId = new OverridableSetting<string>("OAuthAppId", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> authSymmetricKey = new OverridableSetting<string>("OAuthSymmetricKey", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> authCertificateName = new OverridableSetting<string>("OAuthCertificateName", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> streamInsightXamServerName = new OverridableSetting<string>("StreamInsightXamServerName", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> streamInsightXamDatabaseName = new OverridableSetting<string>("StreamInsightXamServerName", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> keynoteDataFeedBaseUrl = new OverridableSetting<string>("KeynoteDataFeedBaseUrl", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> keynoteDataPulseBaseUrl = new OverridableSetting<string>("KeynoteDataPulseBaseUrl", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> keynoteDataFeedAgreementIdUserMapping = new OverridableSetting<string>("KeynoteDataFeedAgreementIdUserMapping", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> keynoteDataFeedBiz40AgreementIdUserMapping = new OverridableSetting<string>("KeynoteDataFeedBiz40AgreementIdUserMapping", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> keynoteDataPulseAgreementIdUserMapping = new OverridableSetting<string>("KeynoteDataPulseAgreementIdUserMapping", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> keynoteDataPulseBiz40AgreementIdUserMapping = new OverridableSetting<string>("KeynoteDataPulseBiz40AgreementIdUserMapping", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> keynoteCredentials = new OverridableSetting<string>("KeynoteCredentials", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> remotePowershellCertSubject = new OverridableSetting<string>("RemotePowershellCertSubject", null, Settings.NoOpConverter, true);

		private static OverridableSetting<int> numberOfLastProbeResults = new OverridableSetting<int>("numberOfLastProbeResults", 5, Settings.Int32Converter, true);

		private static OverridableSetting<int> numberOfLastMonitorResults = new OverridableSetting<int>("numberOfLastMonitorResults", 1, Settings.Int32Converter, true);

		private static OverridableSetting<int> numberOfLastResponderResults = new OverridableSetting<int>("numberOfLastResponderResults", 5, Settings.Int32Converter, true);

		private static OverridableSetting<int> numberOfLastMaintenanceResults = new OverridableSetting<int>("numberOfLastMaintenanceResults", 5, Settings.Int32Converter, true);

		private static OverridableSetting<bool> isPersistentStateEnabled = new OverridableSetting<bool>("IsPersistentStateEnabled", false, Settings.BooleanConverter, true);

		private static OverridableSetting<bool> isCancelWorkItemsOnQuitRequestFeatureEnabled = new OverridableSetting<bool>("IsCancelWorkItemsOnQuitRequestFeatureEnabled", true, Settings.BooleanConverter, true);

		private static OverridableSetting<string> probeSvcAddress = new OverridableSetting<string>("ProbeSvcAddress", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> accessControlHostName = new OverridableSetting<string>("AccessControlHostName", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> accessControlNamespace = new OverridableSetting<string>("AccessControlNamespace", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> accessControlSigningCertificateFilePath = new OverridableSetting<string>("AccessControlSigningCertificateFilePath", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> probeSvcCertificateFilePath = new OverridableSetting<string>("ProbeSvcCertificateFilePath", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> probeSvcCertificateDN = new OverridableSetting<string>("ProbeSvcCertificateDN", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> probeSvcClientUsername = new OverridableSetting<string>("ProbeSvcClientUsername", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> probeSvcClientPassword = new OverridableSetting<string>("ProbeSvcClientPassword", null, Settings.NoOpConverter, true);

		private static OverridableSetting<bool> useSynchronousContinuationForWorkitemResults = new OverridableSetting<bool>("UseSynchronousContinuationForWorkitemResults", true, Settings.BooleanConverter, true);

		private static OverridableSetting<bool> calculateTimeoutFromBeginningOfExecution = new OverridableSetting<bool>("CalculateTimeoutFromBeginningOfExecution", true, Settings.BooleanConverter, true);

		private static OverridableSetting<string> staticMonitoringPassword = new OverridableSetting<string>("StaticMonitoringPassword", null, Settings.NoOpConverter, true);

		private static OverridableSetting<bool> runningAsConsoleHost = new OverridableSetting<bool>("RunningAsConsoleHost", false, Settings.BooleanConverter, true);

		private static OverridableSetting<string> sqlReadOnlyUser = new OverridableSetting<string>("SqlReadOnlyUser", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> sqlReadOnlyPassword = new OverridableSetting<string>("SqlReadOnlyPassword", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> dataInsightSqlReadonlyUsername = new OverridableSetting<string>("DiSqlReadonlyUsername", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> dataInsightSqlReadonlyPassword = new OverridableSetting<string>("DiSqlReadonlyPassword", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> sqlReadWriteUser = new OverridableSetting<string>("SqlReadWriteUser", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> sqlReadWritePassword = new OverridableSetting<string>("SqlReadWritePassword", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> opticsServer = new OverridableSetting<string>("OpticsServer", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> systemMonitoringInstance = new OverridableSetting<string>("SystemMonitoringInstance", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> orcaVdir = new OverridableSetting<string>("OrcaVirtualDirectory", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> orcaUrlSuffix = new OverridableSetting<string>("ORCAUrlSuffix", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> orcaATMUrl = new OverridableSetting<string>("ORCAATMUrl", null, Settings.NoOpConverter, true);

		private static OverridableSetting<string> orcaClientCredential = new OverridableSetting<string>("OrcaClientCredential", null, Settings.NoOpConverter, true);

		private static OverridableSetting<int> batchManagerBatchSize = new OverridableSetting<int>("BatchManagerBatchSize", 1, Settings.Int32Converter, true);

		private static OverridableSetting<int> batchManagerBatchWaitTimeSeconds = new OverridableSetting<int>("BatchManagerBatchWaitTimeSeconds", 60, Settings.Int32Converter, true);

		private static OverridableSetting<bool> useTransactionsInWorkItemGeneration = new OverridableSetting<bool>("UseTransactionsInWorkItemGeneration", true, Settings.BooleanConverter, true);

		private static OverridableSetting<int> cafeMailboxes = new OverridableSetting<int>("CafeMailboxes", 10, Settings.Int32Converter, true);

		private static OverridableSetting<int> maintenanceTimeoutWatsonHours = new OverridableSetting<int>("MaintenanceTimeoutWatsonHours", 0, Settings.Int32Converter, true);

		private static OverridableSetting<bool> isCortex = new OverridableSetting<bool>("IsCortex", false, Settings.BooleanConverter, false);

		private static OverridableSetting<string> azureTableConnectionString = new OverridableSetting<string>("DataConnectionString", null, Settings.NoOpConverter, false);

		private static OverridableSetting<int> maxNumberOfWorkUnits = new OverridableSetting<int>("MaxNumberOfWorkUnits", int.MaxValue, Settings.Int32Converter, true);

		private static OverridableSetting<int> maxWorkUnitCost = new OverridableSetting<int>("MaxWorkUnitCost", int.MaxValue, Settings.Int32Converter, true);

		private static OverridableSetting<int> maxRecoveryAttempts = new OverridableSetting<int>("MaxRecoveryAttempts", 5, Settings.Int32Converter, true);

		private static OverridableSetting<int> recoveryThrottleTimeInSeconds = new OverridableSetting<int>("RecoveryThrottleTimeInSeconds", 900, Settings.Int32Converter, true);

		private static OverridableSetting<bool> restartOnPoisonedWorkItem = new OverridableSetting<bool>("RestartOnPoisonedWorkItem", true, Settings.BooleanConverter, false);

		private static OverridableSetting<string> dataPartition = new OverridableSetting<string>("DataPartition", null, Settings.NoOpConverter, false);

		private static OverridableSetting<string> cortexDataPartitionRingKey = new OverridableSetting<string>("CortexDataPartitionRingKey", null, Settings.NoOpConverter, false);

		private static OverridableSetting<string> orcaHubConnectionString = new OverridableSetting<string>("OrcaHubConnectionString", null, Settings.NoOpConverter, false);

		private static OverridableSetting<string> orcaSpokeConnectionString = new OverridableSetting<string>("OrcaSpokeConnectionString", null, Settings.NoOpConverter, false);

		private static OverridableSetting<string> orcaSpokeIdentity = new OverridableSetting<string>("OrcaSpokeIdentity", null, Settings.NoOpConverter, false);
	}
}
