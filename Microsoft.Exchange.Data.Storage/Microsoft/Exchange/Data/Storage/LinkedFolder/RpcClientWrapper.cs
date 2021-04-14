using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.JobQueue;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
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

		public static EnqueueResult EnqueueTeamMailboxSyncRequest(string targetServer, Guid mailboxGuid, QueueType queueType, OrganizationId orgId, string clientString, string domainController, SyncOption syncOption = SyncOption.Default)
		{
			if (mailboxGuid == Guid.Empty)
			{
				throw new ArgumentNullException("mailboxGuid");
			}
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			if (queueType != QueueType.TeamMailboxDocumentSync && queueType != QueueType.TeamMailboxMembershipSync && queueType != QueueType.TeamMailboxMaintenanceSync)
			{
				throw new ArgumentException("queueType");
			}
			RpcClientWrapper.InitializeIfNeeded();
			if (!RpcClientWrapper.instance.initialized)
			{
				return new EnqueueResult(EnqueueResultType.ClientInitError, ClientStrings.RpcClientInitError);
			}
			EnqueueResult result;
			try
			{
				using (JobQueueRpcClient jobQueueRpcClient = new JobQueueRpcClient(targetServer ?? RpcClientWrapper.instance.localServer.Fqdn))
				{
					TeamMailboxSyncRpcInParameters teamMailboxSyncRpcInParameters = new TeamMailboxSyncRpcInParameters(mailboxGuid, orgId, clientString, syncOption, domainController);
					byte[] data = jobQueueRpcClient.EnqueueRequest(1, (int)queueType, teamMailboxSyncRpcInParameters.Serialize());
					EnqueueRequestRpcOutParameters enqueueRequestRpcOutParameters = new EnqueueRequestRpcOutParameters(data);
					result = enqueueRequestRpcOutParameters.Result;
				}
			}
			catch (RpcException ex)
			{
				result = new EnqueueResult(EnqueueResultType.RpcError, ClientStrings.RpcClientRequestError(ex.Message));
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

		private readonly ITopologyConfigurationSession rootOrgConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 41, "rootOrgConfigSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\LinkedFolder\\RpcClientWrapper.cs");

		private bool initialized;

		private Server localServer;
	}
}
