using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;
using Microsoft.Office.CompliancePolicy;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NotificationLoader
	{
		public NotificationLoader(ExecutionLog logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			this.logger = logger;
		}

		public void EnqueuePendingNotifications()
		{
			UnifiedPolicySyncNotificationDataProvider unifiedPolicySyncNotificationDataProvider = null;
			this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Verbose, "Entering EnqueuePendingNotifications", null);
			foreach (ExchangePrincipal exchangePrincipal in this.GetSyncMailboxPrincipals())
			{
				try
				{
					unifiedPolicySyncNotificationDataProvider = new UnifiedPolicySyncNotificationDataProvider(exchangePrincipal, "NotificationLoader");
				}
				catch (StoragePermanentException exception)
				{
					this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Error, "EnqueuePendingNotifications: UnifiedPolicySyncNotificationDataProvider ctor failed with StoragePermanentException for " + exchangePrincipal.MailboxInfo.PrimarySmtpAddress, exception);
					continue;
				}
				catch (StorageTransientException exception2)
				{
					this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Error, "EnqueuePendingNotifications: UnifiedPolicySyncNotificationDataProvider ctor failed with StorageTransientException for " + exchangePrincipal.MailboxInfo.PrimarySmtpAddress, exception2);
					continue;
				}
				using (unifiedPolicySyncNotificationDataProvider)
				{
					try
					{
						IEnumerable<UnifiedPolicyNotificationBase> enumerable = unifiedPolicySyncNotificationDataProvider.FindPaged<UnifiedPolicyNotificationBase>(null, null, false, null, 0);
						this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Verbose, string.Format("EnqueuePendingNotifications: found {0} pending notifications on system mailbox {1}", (enumerable != null) ? enumerable.Count<UnifiedPolicyNotificationBase>() : 0, exchangePrincipal.MailboxInfo.PrimarySmtpAddress), null);
						foreach (UnifiedPolicyNotificationBase unifiedPolicyNotificationBase in enumerable)
						{
							try
							{
								this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Verbose, string.Format("EnqueuePendingNotifications: ready to dispatch workitemId: {0}", unifiedPolicyNotificationBase.Identity), null);
								this.EnqueueWorkItem(unifiedPolicyNotificationBase);
							}
							catch (SyncAgentExceptionBase)
							{
								this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Error, string.Format("EnqueuePendingNotifications: EnqueueWorkItem failed to dispatch workitemId: {0}", unifiedPolicyNotificationBase.Identity), null);
							}
						}
					}
					catch (StoragePermanentException exception3)
					{
						this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Error, "EnqueuePendingNotifications: dataProvider.FindPaged failed with StoragePermanentException for " + exchangePrincipal.MailboxInfo.PrimarySmtpAddress, exception3);
					}
					catch (StorageTransientException exception4)
					{
						this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Error, "EnqueuePendingNotifications: dataProvider.FindPaged failed with StorageTransientException for " + exchangePrincipal.MailboxInfo.PrimarySmtpAddress, exception4);
					}
				}
			}
			this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Verbose, "Exiting EnqueuePendingNotifications", null);
		}

		protected virtual void EnqueueWorkItem(UnifiedPolicyNotificationBase notification)
		{
			SyncManager.EnqueueWorkItem(notification.GetWorkItem());
		}

		private IEnumerable<ExchangePrincipal> GetSyncMailboxPrincipals()
		{
			IDirectorySession session = null;
			foreach (Guid database in this.FindLocalDatabases())
			{
				if (session == null)
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
					session = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 191, "GetSyncMailboxPrincipals", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\UnifiedPolicy\\NotificationLoader.cs");
				}
				foreach (ExchangePrincipal principal in this.GetSyncMailboxPrincipals(database, session))
				{
					yield return principal;
				}
			}
			yield break;
		}

		private IList<ExchangePrincipal> GetSyncMailboxPrincipals(Guid mailboxDatabaseGuid, IDirectorySession configSession)
		{
			List<ExchangePrincipal> list = new List<ExchangePrincipal>();
			ADObjectId id = new ADObjectId(mailboxDatabaseGuid);
			Result<MailboxDatabase>[] dataBases = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				dataBases = configSession.FindByADObjectIds<MailboxDatabase>(new ADObjectId[]
				{
					id
				});
			}, 3);
			if (!adoperationResult.Succeeded)
			{
				this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Error, string.Format("GetSyncMailboxPrincipals: failed to find database {0} in active directory because of {1}", mailboxDatabaseGuid, adoperationResult.Exception), adoperationResult.Exception);
			}
			if (dataBases != null && dataBases.Length > 0)
			{
				PartitionId[] allAccountPartitionIds = ADAccountPartitionLocator.GetAllAccountPartitionIds();
				for (int i = 0; i < allAccountPartitionIds.Length; i++)
				{
					PartitionId partitionId = allAccountPartitionIds[i];
					IRecipientSession recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 238, "GetSyncMailboxPrincipals", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\UnifiedPolicy\\NotificationLoader.cs");
					ADUser[] arbMbxs = null;
					adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						arbMbxs = recipientSession.FindPaged<ADUser>(RecipientFilterHelper.DiscoveryMailboxFilterUnifiedPolicy(dataBases[0].Data.Id), null, true, null, 0).ToArray<ADUser>();
					}, 3);
					if (!adoperationResult.Succeeded)
					{
						this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Error, string.Format("GetSyncMailboxPrincipals: failed to find sync mailboxes in database {0} in active directory because of {1}", mailboxDatabaseGuid, adoperationResult.Exception), adoperationResult.Exception);
					}
					if (arbMbxs != null && arbMbxs.Length > 0)
					{
						ADUser[] arbMbxs2 = arbMbxs;
						int j = 0;
						while (j < arbMbxs2.Length)
						{
							ADUser aduser = arbMbxs2[j];
							this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Verbose, string.Format("GetSyncMailboxPrincipals: found sync mailbox {0} in database {1} in partition {2}", aduser.UserPrincipalName, mailboxDatabaseGuid, partitionId), null);
							ExchangePrincipal item = null;
							try
							{
								item = ExchangePrincipal.FromADUser(ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(aduser.OrganizationId), aduser, RemotingOptions.LocalConnectionsOnly);
							}
							catch (StoragePermanentException exception)
							{
								this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Error, string.Format("GetSyncMailboxPrincipals: sync mailbox {0} is skipped because of StoragePermanentException in ExchangePrincipal construction", aduser.UserPrincipalName), exception);
								goto IL_20F;
							}
							catch (StorageTransientException exception2)
							{
								this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Error, string.Format("GetSyncMailboxPrincipals: sync mailbox {0} is skipped because of StorageTransientException in ExchangePrincipal construction", aduser.UserPrincipalName), exception2);
								goto IL_20F;
							}
							goto IL_207;
							IL_20F:
							j++;
							continue;
							IL_207:
							list.Add(item);
							goto IL_20F;
						}
					}
					else
					{
						this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Verbose, string.Format("GetSyncMailboxPrincipals: there is no sync mailboxes in database {0} in active directory", mailboxDatabaseGuid), null);
					}
				}
			}
			return list;
		}

		private Server GetLocalServer()
		{
			if (this.localServer == null)
			{
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 345, "GetLocalServer", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\UnifiedPolicy\\NotificationLoader.cs");
					this.localServer = topologyConfigurationSession.FindLocalServer();
				});
				if (!adoperationResult.Succeeded)
				{
					this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Error, "GetLocalServer: failed", adoperationResult.Exception);
					throw adoperationResult.Exception;
				}
			}
			return this.localServer;
		}

		private IList<Guid> FindLocalDatabases()
		{
			IList<Guid> list = this.FindLocalDatabasesFromAdminRPC();
			if (list.Count == 0)
			{
				this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Verbose, "FindLocalDatabases: FindLocalDatabasesFromAdminRPC didn't return any databases. Try FindLocalDatabasesFromAD.", null);
				list = this.FindLocalDatabasesFromAD();
			}
			return list;
		}

		private IList<Guid> FindLocalDatabasesFromAdminRPC()
		{
			IList<Guid> list = new List<Guid>();
			try
			{
				Server localServer = null;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					localServer = this.GetLocalServer();
				});
				if (adoperationResult.ErrorCode != ADOperationErrorCode.Success || localServer == null)
				{
					return list;
				}
				using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=UnifiedPolicy", localServer.Name, null, null, null))
				{
					MdbStatus[] array = exRpcAdmin.ListMdbStatus(true);
					if (array == null || array.Length == 0)
					{
						return list;
					}
					foreach (MdbStatus mdbStatus in array)
					{
						bool flag = (mdbStatus.Status & MdbStatusFlags.Online) == MdbStatusFlags.Online;
						if (flag)
						{
							list.Add(mdbStatus.MdbGuid);
						}
						else
						{
							this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Verbose, string.Format("FindLocalDatabasesFromAdminRPC: skip offline database {0}", mdbStatus.MdbGuid), null);
						}
					}
				}
			}
			catch (MapiPermanentException exception)
			{
				this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Error, "FindLocalDatabasesFromAdminRPC: failed with MapiPermanentException", exception);
			}
			catch (MapiRetryableException exception2)
			{
				this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Error, "FindLocalDatabasesFromAdminRPC: failed with MapiRetryableException", exception2);
			}
			this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Verbose, string.Format("FindLocalDatabasesFromAdminRPC: found {0} database on local server", list.Count), null);
			return list;
		}

		private IList<Guid> FindLocalDatabasesFromAD()
		{
			MailboxDatabase[] databases = null;
			IList<Guid> list = new List<Guid>();
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				databases = this.GetLocalServer().GetMailboxDatabases();
			});
			if (adoperationResult.ErrorCode != ADOperationErrorCode.Success || databases == null)
			{
				return list;
			}
			foreach (MailboxDatabase mailboxDatabase in databases)
			{
				bool flag = mailboxDatabase.IsValid && mailboxDatabase.Mounted != null && mailboxDatabase.Mounted.Value;
				if (flag)
				{
					list.Add(mailboxDatabase.Guid);
				}
				else
				{
					this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Verbose, string.Format("FindLocalDatabasesFromAD: skip offline database {0}", mailboxDatabase.Guid), null);
				}
			}
			this.logger.LogOneEntry("NotificationLoader", string.Empty, ExecutionLog.EventType.Verbose, string.Format("FindLocalDatabasesFromAD: found {0} database on local server", list.Count), null);
			return list;
		}

		private const string clientName = "NotificationLoader";

		private volatile Server localServer;

		private readonly ExecutionLog logger;
	}
}
