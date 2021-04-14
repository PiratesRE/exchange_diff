using System;
using System.Collections.Generic;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Monitor
{
	public static class PerfCounters
	{
		public static Dictionary<ConfigurationObjectType, KeyValuePair<string, string>> PolicyObjectWsPerfCounters
		{
			get
			{
				return PerfCounters.policyObjectWsPerfCounters;
			}
		}

		public static Dictionary<ConfigurationObjectType, KeyValuePair<string, string>> PolicyObjectCrudMgrPerfCounters
		{
			get
			{
				return PerfCounters.policyObjectCrudMgrPerfCounters;
			}
		}

		public const string RecommendedCategoryName = "MSUnified Compliance Sync";

		public const string TotalProcessingTimePerSyncRequest = "Total Processing Time Per Sync Request";

		public const string TotalProcessingTimePerSyncRequestBase = "Total Processing Time Per Sync Request Base";

		public const string ExecutionDelayTimePerSyncRequest = "Execution Delay Time Per Sync Request";

		public const string ExecutionDelayTimePerSyncRequestBase = "Execution Delay Time Per Sync Request Base";

		public const string InitializationTimePerSyncRequest = "Initialization Time Per Sync Request";

		public const string InitializationTimePerSyncRequestBase = "Initialization Time Per Sync Request Base";

		public const string TenantInfoProcessingTimePerSyncRequest = "TenantInfo Processing Time Per Sync Request";

		public const string TenantInfoProcessingTimePerSyncRequestBase = "TenantInfo Processing Time Per Sync Request Base";

		public const string PersistentQueueProcessingTimePerSyncRequest = "Persistent Queue Processing Time Per Sync Request";

		public const string PersistentQueueProcessingTimePerSyncRequestBase = "Persistent Queue Processing Time Per Sync Request Base";

		public const string ProcessedSyncRequestNumberPerSecond = "Processed Sync Request Number Per Second";

		public const string ProcessedSyncRequestNumber = "Processed Sync Request Number";

		public const string SuccessfulSyncRequestNumberPerSecond = "Successful Sync Request Number Per Second";

		public const string SuccessfulSyncRequestNumber = "Successful Sync Request Number";

		public const string PolicySyncWsCallTransientErrorNumberPerSecond = "Policy Sync Ws Call Transient Error Number Per Second";

		public const string PolicySyncWsCallTransientErrorNumber = "Policy Sync Ws Call Transient Error Number";

		public const string PolicySyncCrudMgrTransientErrorNumberPerSecond = "Policy Sync CrudMgr Transient Error Number Per Second";

		public const string PolicySyncCrudMgrTransientErrorNumber = "Policy Sync CrudMgr Transient Error Number";

		public const string PolicySyncWsCallPermanentErrorNumberPerSecond = "Policy Sync Ws Call Permanent Error Number Per Second";

		public const string PolicySyncWsCallPermanentErrorNumber = "Policy Sync Ws Call Permanent Error Number";

		public const string PolicySyncCrudMgrPermanentErrorNumberPerSecond = "Policy Sync CrudMgr Permanent Error Number Per Second";

		public const string PolicySyncCrudMgrPermanentErrorNumber = "Policy Sync CrudMgr Permanent Error Number";

		public const string StatusUpdatePermanentErrorNumberPerSecond = "Status Update Permanent Error Number Per Second";

		public const string StatusUpdatePermanentErrorNumber = "Status Update Permanent Error Number";

		public const string SyncRequestRetryNumberPerSecond = "Sync Request Retry Number Per Second";

		public const string SyncRequestRetryNumber = "Sync Request Retry Number";

		public const string WsCallNumberPerSyncRequest = "Ws Call Number Per Sync Request";

		public const string WsCallNumberPerSyncRequestBase = "Ws Call Number Per Sync Request Base";

		public const string TenantNumberPerSyncRequest = "Tenant Number Per Sync Request";

		public const string TenantNumberPerSyncRequestBase = "Tenant Number Per Sync Request Base";

		public const string ObjectNumberPerSyncRequest = "Object Number Per Sync Request";

		public const string ObjectNumberPerSyncRequestBase = "Object Number Per Sync Request Base";

		public const string DarTasksTransientFailed = "DarTasksTransientFailed";

		public const string DarTasksInStateNone = "DarTasksInStateNone";

		public const string DarTasksInStateReady = "DarTasksInStateReady";

		public const string DarTasksInStateRunning = "DarTasksInStateRunning";

		public const string DarTasksInStateCompleted = "DarTasksInStateCompleted";

		public const string DarTasksInStateFailed = "DarTasksInStateFailed";

		public const string DarTasksInStateCancelled = "DarTasksInStateCancelled";

		public const string DarTaskAverageDuration = "DarTaskAverageDuration";

		public const string DarTaskAverageDurationBase = "DarTaskAverageDurationBase";

		private static Dictionary<ConfigurationObjectType, KeyValuePair<string, string>> policyObjectWsPerfCounters = new Dictionary<ConfigurationObjectType, KeyValuePair<string, string>>
		{
			{
				ConfigurationObjectType.Policy,
				new KeyValuePair<string, string>("Ws Call Time For Policy Per Sync Request", "Ws Call Time For Policy Per Sync Request Base")
			},
			{
				ConfigurationObjectType.Rule,
				new KeyValuePair<string, string>("Ws Call Time For Rule Per Sync Request", "Ws Call Time For Rule Per Sync Request Base")
			},
			{
				ConfigurationObjectType.Binding,
				new KeyValuePair<string, string>("Ws Call Time For Binding Per Sync Request", "Ws Call Time For Binding Per Sync Request Base")
			},
			{
				ConfigurationObjectType.Association,
				new KeyValuePair<string, string>("Ws Call Time For Association Per Sync Request", "Ws Call Time For Association Per Sync Request Base")
			}
		};

		private static Dictionary<ConfigurationObjectType, KeyValuePair<string, string>> policyObjectCrudMgrPerfCounters = new Dictionary<ConfigurationObjectType, KeyValuePair<string, string>>
		{
			{
				ConfigurationObjectType.Policy,
				new KeyValuePair<string, string>("CrudMgr Time For Policy Per Sync Request", "CrudMgr Time For Policy Per Sync Request Base")
			},
			{
				ConfigurationObjectType.Rule,
				new KeyValuePair<string, string>("CrudMgr Time For Rule Per Sync Request", "CrudMgr Time For Rule Per Sync Request Base")
			},
			{
				ConfigurationObjectType.Binding,
				new KeyValuePair<string, string>("CrudMgr Time For Binding Per Sync Request", "CrudMgr Time For Binding Per Sync Request Base")
			},
			{
				ConfigurationObjectType.Association,
				new KeyValuePair<string, string>("CrudMgr Time For Association Per Sync Request", "CrudMgr Time For Association Per Sync Request Base")
			}
		};
	}
}
