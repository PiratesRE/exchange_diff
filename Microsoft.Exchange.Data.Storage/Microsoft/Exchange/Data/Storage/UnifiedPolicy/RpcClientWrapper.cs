using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.UnifiedPolicyNotification;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RpcClientWrapper
	{
		private RpcClientWrapper()
		{
		}

		private static void InitializeIfNeeded()
		{
			if (!RpcClientWrapper.instance.initialized)
			{
				lock (RpcClientWrapper.instance.initializeLockObject)
				{
					if (!RpcClientWrapper.instance.initialized)
					{
						RpcClientWrapper.instance.Initialize();
					}
				}
			}
		}

		public static SyncNotificationResult NotifySyncChanges(string identity, string targetServerFqdn, Guid tenantId, string syncSvcUrl, bool fullSync, bool syncNow, SyncChangeInfo[] syncChangeInfos)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			if (tenantId == Guid.Empty)
			{
				throw new ArgumentNullException("tenantId");
			}
			if (string.IsNullOrEmpty(syncSvcUrl))
			{
				throw new ArgumentNullException("syncSvcUrl");
			}
			RpcClientWrapper.InitializeIfNeeded();
			NotificationType type = NotificationType.Sync;
			if (!RpcClientWrapper.instance.initialized)
			{
				return new SyncNotificationResult(new SyncAgentTransientException("RPC client not initialized"));
			}
			SyncNotificationResult result;
			try
			{
				using (UnifiedPolicyNotificationRpcClient unifiedPolicyNotificationRpcClient = new UnifiedPolicyNotificationRpcClient(targetServerFqdn ?? RpcClientWrapper.instance.localServer.Fqdn))
				{
					SyncWorkItem syncWorkItem = new SyncWorkItem(identity, syncNow, new TenantContext(tenantId, null), syncChangeInfos, syncSvcUrl, fullSync, Workload.Exchange, false);
					byte[] data = unifiedPolicyNotificationRpcClient.Notify(1, (int)type, syncWorkItem.Serialize());
					NotificationRpcOutParameters notificationRpcOutParameters = new NotificationRpcOutParameters(data);
					result = notificationRpcOutParameters.Result;
				}
			}
			catch (RpcException ex)
			{
				result = new SyncNotificationResult(new SyncAgentTransientException(ex.Message, ex));
			}
			return result;
		}

		public static SyncNotificationResult NotifyStatusChanges(string identity, string targetServerFqdn, Guid tenantId, string statusUpdateSvcUrl, bool syncNow, IList<UnifiedPolicyStatus> statusUpdates)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			if (tenantId == Guid.Empty)
			{
				throw new ArgumentNullException("tenantId");
			}
			if (string.IsNullOrEmpty(statusUpdateSvcUrl))
			{
				throw new ArgumentNullException("statusUpdateSvcUrl");
			}
			if (statusUpdates == null || statusUpdates.Count == 0)
			{
				throw new ArgumentNullException("statusUpdates");
			}
			RpcClientWrapper.InitializeIfNeeded();
			NotificationType type = NotificationType.ApplicationStatus;
			if (!RpcClientWrapper.instance.initialized)
			{
				return new SyncNotificationResult(new SyncAgentTransientException("RPC client not initialized"));
			}
			SyncNotificationResult result;
			try
			{
				using (UnifiedPolicyNotificationRpcClient unifiedPolicyNotificationRpcClient = new UnifiedPolicyNotificationRpcClient(targetServerFqdn ?? RpcClientWrapper.instance.localServer.Fqdn))
				{
					SyncStatusUpdateWorkitem syncStatusUpdateWorkitem = new SyncStatusUpdateWorkitem(identity, syncNow, new TenantContext(tenantId, null), statusUpdates, statusUpdateSvcUrl, 0);
					byte[] data = unifiedPolicyNotificationRpcClient.Notify(1, (int)type, syncStatusUpdateWorkitem.Serialize());
					NotificationRpcOutParameters notificationRpcOutParameters = new NotificationRpcOutParameters(data);
					result = notificationRpcOutParameters.Result;
				}
			}
			catch (RpcException ex)
			{
				result = new SyncNotificationResult(new SyncAgentTransientException(ex.Message, ex));
			}
			return result;
		}

		private void Initialize()
		{
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				this.localServer = this.rootOrgConfigSession.FindLocalServer();
			}, 3);
			if (adoperationResult.Succeeded)
			{
				this.initialized = true;
			}
		}

		private const int CurrentAPIVersion = 1;

		private static readonly RpcClientWrapper instance = new RpcClientWrapper();

		private readonly object initializeLockObject = new object();

		private readonly ITopologyConfigurationSession rootOrgConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 44, "rootOrgConfigSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\UnifiedPolicy\\RpcClientWrapper.cs");

		private bool initialized;

		private Server localServer;
	}
}
