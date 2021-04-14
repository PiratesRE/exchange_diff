using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.JobQueue;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class PublicFolderSyncJobRpc
	{
		public static PublicFolderSyncJobState SyncFolder(IExchangePrincipal contentMailboxPrincipal, byte[] folderId)
		{
			ArgumentValidator.ThrowIfNull("contentMailboxPrincipal", contentMailboxPrincipal);
			return PublicFolderSyncJobRpc.SyncFolder(contentMailboxPrincipal.MailboxInfo.OrganizationId, contentMailboxPrincipal.MailboxInfo.MailboxGuid, contentMailboxPrincipal.MailboxInfo.Location.ServerFqdn, folderId);
		}

		public static PublicFolderSyncJobState SyncFolder(OrganizationId organizationId, Guid mailboxGuid, string serverFqdn, byte[] folderId)
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			ArgumentValidator.ThrowIfEmpty("mailboxGuid", mailboxGuid);
			ArgumentValidator.ThrowIfNull("server", serverFqdn);
			ArgumentValidator.ThrowIfNull("folderId", folderId);
			ArgumentValidator.ThrowIfZeroOrNegative("folderId.Length", folderId.Length);
			PublicFolderSyncJobState publicFolderSyncJobState;
			try
			{
				using (JobQueueRpcClient jobQueueRpcClient = new JobQueueRpcClient(serverFqdn))
				{
					PublicFolderSyncJobRpcInParameters publicFolderSyncJobRpcInParameters = new PublicFolderSyncJobRpcInParameters(organizationId, mailboxGuid, folderId);
					byte[] data = jobQueueRpcClient.EnqueueRequest(1, 1, publicFolderSyncJobRpcInParameters.Serialize());
					EnqueueRequestRpcOutParameters enqueueRequestRpcOutParameters = new EnqueueRequestRpcOutParameters(data);
					PublicFolderSyncJobEnqueueResult publicFolderSyncJobEnqueueResult = enqueueRequestRpcOutParameters.Result as PublicFolderSyncJobEnqueueResult;
					if (publicFolderSyncJobEnqueueResult == null || publicFolderSyncJobEnqueueResult.PublicFolderSyncJobState == null)
					{
						throw new PublicFolderSyncPermanentException(ServerStrings.PublicFolderSyncFolderRpcFailed);
					}
					publicFolderSyncJobState = publicFolderSyncJobEnqueueResult.PublicFolderSyncJobState;
				}
			}
			catch (RpcException innerException)
			{
				throw new PublicFolderSyncTransientException(ServerStrings.PublicFolderSyncFolderRpcFailed, innerException);
			}
			catch (SerializationException innerException2)
			{
				throw new PublicFolderSyncPermanentException(ServerStrings.PublicFolderSyncFolderRpcFailed, innerException2);
			}
			return publicFolderSyncJobState;
		}

		public static PublicFolderSyncJobState StartSyncHierarchy(IExchangePrincipal contentMailboxPrincipal, bool executeReconcileFolders = false)
		{
			ArgumentValidator.ThrowIfNull("contentMailboxPrincipal", contentMailboxPrincipal);
			return PublicFolderSyncJobRpc.StartSyncHierarchy(contentMailboxPrincipal.MailboxInfo.OrganizationId, contentMailboxPrincipal.MailboxInfo.MailboxGuid, contentMailboxPrincipal.MailboxInfo.Location.ServerFqdn, executeReconcileFolders);
		}

		public static PublicFolderSyncJobState StartSyncHierarchy(OrganizationId organizationId, Guid mailboxGuid, string serverFqdn, bool executeReconcileFolders = false)
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			ArgumentValidator.ThrowIfEmpty("mailboxGuid", mailboxGuid);
			ArgumentValidator.ThrowIfNull("server", serverFqdn);
			PublicFolderSyncJobState publicFolderSyncJobState;
			try
			{
				using (JobQueueRpcClient jobQueueRpcClient = new JobQueueRpcClient(serverFqdn))
				{
					PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction syncAction = executeReconcileFolders ? PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction.StartSyncHierarchyWithFolderReconciliation : PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction.StartSyncHierarchy;
					PublicFolderSyncJobRpcInParameters publicFolderSyncJobRpcInParameters = new PublicFolderSyncJobRpcInParameters(organizationId, mailboxGuid, syncAction);
					byte[] data = jobQueueRpcClient.EnqueueRequest(1, 1, publicFolderSyncJobRpcInParameters.Serialize());
					EnqueueRequestRpcOutParameters enqueueRequestRpcOutParameters = new EnqueueRequestRpcOutParameters(data);
					PublicFolderSyncJobEnqueueResult publicFolderSyncJobEnqueueResult = enqueueRequestRpcOutParameters.Result as PublicFolderSyncJobEnqueueResult;
					if (publicFolderSyncJobEnqueueResult == null || publicFolderSyncJobEnqueueResult.PublicFolderSyncJobState == null)
					{
						throw new PublicFolderSyncPermanentException(ServerStrings.PublicFolderStartSyncFolderHierarchyRpcFailed);
					}
					publicFolderSyncJobState = publicFolderSyncJobEnqueueResult.PublicFolderSyncJobState;
				}
			}
			catch (RpcException innerException)
			{
				throw new PublicFolderSyncTransientException(ServerStrings.PublicFolderStartSyncFolderHierarchyRpcFailed, innerException);
			}
			catch (SerializationException innerException2)
			{
				throw new PublicFolderSyncPermanentException(ServerStrings.PublicFolderStartSyncFolderHierarchyRpcFailed, innerException2);
			}
			return publicFolderSyncJobState;
		}

		public static PublicFolderSyncJobState QueryStatusSyncHierarchy(IExchangePrincipal contentMailboxPrincipal)
		{
			ArgumentValidator.ThrowIfNull("contentMailboxPrincipal", contentMailboxPrincipal);
			return PublicFolderSyncJobRpc.QueryStatusSyncHierarchy(contentMailboxPrincipal.MailboxInfo.OrganizationId, contentMailboxPrincipal.MailboxInfo.MailboxGuid, contentMailboxPrincipal.MailboxInfo.Location.ServerFqdn);
		}

		public static PublicFolderSyncJobState QueryStatusSyncHierarchy(OrganizationId organizationId, Guid mailboxGuid, string serverFqdn)
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			ArgumentValidator.ThrowIfEmpty("mailboxGuid", mailboxGuid);
			ArgumentValidator.ThrowIfNull("server", serverFqdn);
			PublicFolderSyncJobState publicFolderSyncJobState;
			try
			{
				using (JobQueueRpcClient jobQueueRpcClient = new JobQueueRpcClient(serverFqdn))
				{
					PublicFolderSyncJobRpcInParameters publicFolderSyncJobRpcInParameters = new PublicFolderSyncJobRpcInParameters(organizationId, mailboxGuid, PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction.QueryStatusSyncHierarchy);
					byte[] data = jobQueueRpcClient.EnqueueRequest(1, 1, publicFolderSyncJobRpcInParameters.Serialize());
					EnqueueRequestRpcOutParameters enqueueRequestRpcOutParameters = new EnqueueRequestRpcOutParameters(data);
					PublicFolderSyncJobEnqueueResult publicFolderSyncJobEnqueueResult = enqueueRequestRpcOutParameters.Result as PublicFolderSyncJobEnqueueResult;
					if (publicFolderSyncJobEnqueueResult == null || publicFolderSyncJobEnqueueResult.PublicFolderSyncJobState == null)
					{
						throw new PublicFolderSyncPermanentException(ServerStrings.PublicFolderQueryStatusSyncFolderHierarchyRpcFailed);
					}
					publicFolderSyncJobState = publicFolderSyncJobEnqueueResult.PublicFolderSyncJobState;
				}
			}
			catch (RpcException innerException)
			{
				throw new PublicFolderSyncTransientException(ServerStrings.PublicFolderQueryStatusSyncFolderHierarchyRpcFailed, innerException);
			}
			catch (SerializationException innerException2)
			{
				throw new PublicFolderSyncPermanentException(ServerStrings.PublicFolderQueryStatusSyncFolderHierarchyRpcFailed, innerException2);
			}
			return publicFolderSyncJobState;
		}

		private const int CurrentAPIVersion = 1;
	}
}
