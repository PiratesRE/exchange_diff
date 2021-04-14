using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PersistentSyncWorkItemQueueProvider : IWorkItemQueueProvider
	{
		public void Enqueue(WorkItemBase item)
		{
			if (item == null)
			{
				throw new ArgumentException("item is null");
			}
			try
			{
				using (UnifiedPolicySyncNotificationDataProvider dataProvider = this.GetDataProvider(item.TenantContext.TenantId))
				{
					UnifiedPolicyNotificationBase unifiedPolicyNotificationBase = UnifiedPolicyNotificationFactory.Create(item, dataProvider.MailboxOwner.Id);
					dataProvider.Save(unifiedPolicyNotificationBase);
					item.HasPersistentBackUp = true;
					item.WorkItemId = unifiedPolicyNotificationBase.StoreObjectId.ToBase64String();
				}
			}
			catch (StorageTransientException innerException)
			{
				throw new SyncAgentTransientException("Failed to enqueue sync workitem into persistent queue because of StorageTransientException", innerException);
			}
			catch (StoragePermanentException innerException2)
			{
				throw new SyncAgentPermanentException("Failed to enqueue sync workitem into persistent queue because of StoragePermanentException", innerException2);
			}
			catch (TransientException innerException3)
			{
				throw new SyncAgentTransientException("Failed to enqueue sync workitem into persistent queue because of TransientException", innerException3);
			}
			catch (DataSourceOperationException innerException4)
			{
				throw new SyncAgentTransientException("Failed to enqueue sync workitem into persistent queue because of DataSourceOperationException", innerException4);
			}
			catch (DataValidationException innerException5)
			{
				throw new SyncAgentTransientException("Failed to enqueue sync workitem into persistent queue because of DataValidationException", innerException5);
			}
		}

		public IList<WorkItemBase> Dequeue(int maxCount)
		{
			throw new NotImplementedException("Dequeue not implemented by persistent queue");
		}

		public IList<WorkItemBase> GetAll()
		{
			return null;
		}

		public bool IsEmpty()
		{
			IList<WorkItemBase> all = this.GetAll();
			return all == null || all.Count == 0;
		}

		public void Update(WorkItemBase item)
		{
			if (item == null)
			{
				throw new ArgumentException("item is null");
			}
			if (item.WorkItemId == null)
			{
				throw new ArgumentException("WorkItemId can't be null inside Update");
			}
			try
			{
				using (UnifiedPolicySyncNotificationDataProvider dataProvider = this.GetDataProvider(item.TenantContext.TenantId))
				{
					UnifiedPolicyNotificationBase unifiedPolicyNotificationBase = UnifiedPolicyNotificationFactory.Create(item, dataProvider.MailboxOwner.Id);
					unifiedPolicyNotificationBase.ResetChangeTracking(true);
					unifiedPolicyNotificationBase.Version = Guid.NewGuid();
					dataProvider.Save(unifiedPolicyNotificationBase);
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			catch (StorageTransientException innerException)
			{
				throw new SyncAgentTransientException("Failed to update sync workitem in persistent queue because of StorageTransientException", innerException);
			}
			catch (StoragePermanentException innerException2)
			{
				throw new SyncAgentPermanentException("Failed to update sync workitem in persistent queue because of StoragePermanentException", innerException2);
			}
			catch (TransientException innerException3)
			{
				throw new SyncAgentTransientException("Failed to update sync workitem in persistent queue because of TransientException", innerException3);
			}
			catch (DataSourceOperationException innerException4)
			{
				throw new SyncAgentTransientException("Failed to update sync workitem in persistent queue because of DataSourceOperationException", innerException4);
			}
			catch (DataValidationException innerException5)
			{
				throw new SyncAgentTransientException("Failed to update sync workitem in persistent queue because of DataValidationException", innerException5);
			}
		}

		public WorkItemBase Get(ObjectId identity, Guid tenantId)
		{
			try
			{
				using (UnifiedPolicySyncNotificationDataProvider dataProvider = this.GetDataProvider(tenantId))
				{
					UnifiedPolicyNotificationBase[] array = (UnifiedPolicyNotificationBase[])dataProvider.Find<UnifiedPolicyNotificationBase>(null, identity, false, null);
					if (array != null && array.Length > 0)
					{
						return array[0].GetWorkItem();
					}
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			catch (StorageTransientException innerException)
			{
				throw new SyncAgentTransientException("Failed to get sync workitem in persistent queue because of StorageTransientException", innerException);
			}
			catch (StoragePermanentException innerException2)
			{
				throw new SyncAgentPermanentException("Failed to get sync workitem in persistent queue because of StoragePermanentException", innerException2);
			}
			catch (TransientException innerException3)
			{
				throw new SyncAgentTransientException("Failed to get sync workitem in persistent queue because of TransientException", innerException3);
			}
			catch (DataSourceOperationException innerException4)
			{
				throw new SyncAgentTransientException("Failed to get sync workitem in persistent queue because of DataSourceOperationException", innerException4);
			}
			catch (DataValidationException innerException5)
			{
				throw new SyncAgentTransientException("Failed to get sync workitem in persistent queue because of DataValidationException", innerException5);
			}
			return null;
		}

		public void Delete(WorkItemBase item)
		{
			if (item == null)
			{
				throw new ArgumentException("item is null");
			}
			try
			{
				VersionedId rootId = VersionedId.Deserialize(item.WorkItemId);
				using (UnifiedPolicySyncNotificationDataProvider dataProvider = this.GetDataProvider(item.TenantContext.TenantId))
				{
					UnifiedPolicyNotificationBase[] array = (UnifiedPolicyNotificationBase[])dataProvider.Find<UnifiedPolicyNotificationBase>(null, rootId, false, null);
					if (array != null && array.Length > 0)
					{
						dataProvider.Delete(array[0]);
					}
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			catch (StorageTransientException innerException)
			{
				throw new SyncAgentTransientException("Failed to delete sync workitem from persistent queue because of StorageTransientException", innerException);
			}
			catch (StoragePermanentException innerException2)
			{
				throw new SyncAgentPermanentException("Failed to delete sync workitem from persistent queue because of StoragePermanentException", innerException2);
			}
			catch (TransientException innerException3)
			{
				throw new SyncAgentTransientException("Failed to delete sync workitem from persistent queue because of TransientException", innerException3);
			}
			catch (DataSourceOperationException innerException4)
			{
				throw new SyncAgentPermanentException("Failed to delete sync workitem from persistent queue because of DataSourceOperationException", innerException4);
			}
			catch (DataValidationException innerException5)
			{
				throw new SyncAgentPermanentException("Failed to delete sync workitem from persistent queue because of DataValidationException", innerException5);
			}
		}

		public void OnWorkItemCompleted(WorkItemBase item)
		{
			throw new NotImplementedException("OnWorkItemCompleted not implemented by persistent queue");
		}

		public void OnAllWorkItemDispatched()
		{
			throw new NotImplementedException("OnAllWorkItemDispatched not implemented by persistent queue");
		}

		private UnifiedPolicySyncNotificationDataProvider GetDataProvider(Guid tenantId)
		{
			ADUser mailboxOwner = this.syncArbitrationMailboxADObjectCache.Get(tenantId);
			return new UnifiedPolicySyncNotificationDataProvider(ADSessionSettings.FromExternalDirectoryOrganizationId(tenantId), mailboxOwner, "New-CompliancePolicySyncNotification");
		}

		private readonly SyncArbitrationMailboxADObjectCache syncArbitrationMailboxADObjectCache = new SyncArbitrationMailboxADObjectCache();
	}
}
