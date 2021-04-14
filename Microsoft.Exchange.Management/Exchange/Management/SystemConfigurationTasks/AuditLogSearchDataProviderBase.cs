using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class AuditLogSearchDataProviderBase : XsoMailboxDataProviderBase
	{
		protected AuditLogSearchDataProviderBase(MailboxSession mailboxSession, AuditLogSearchDataProviderBase.SearchSetting setting) : base(mailboxSession)
		{
			this.setting = setting;
			this.folder = this.GetSearchRequestFolder();
		}

		public Folder Folder
		{
			get
			{
				return this.folder;
			}
		}

		internal static MailboxSession GetMailboxSession(OrganizationId organizationId, string action)
		{
			ADSessionSettings sessionSettings = organizationId.ToADSessionSettings();
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.FullyConsistent, sessionSettings, 99, "GetMailboxSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\AuditLogSearch\\AuditLogSearchDataProvider.cs");
			ADUser discoveryMailbox = MailboxDataProvider.GetDiscoveryMailbox(tenantOrRootOrgRecipientSession);
			return AuditLogSearchDataProviderBase.GetMailboxSession(discoveryMailbox, action);
		}

		internal static MailboxSession GetMailboxSession(ADUser mailbox, string action)
		{
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(mailbox, RemotingOptions.AllowCrossSite);
			return MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, "Client=Management;Action=" + action);
		}

		private Folder GetSearchRequestFolder()
		{
			StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Root);
			string folderName = this.setting.FolderName;
			Folder folder = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				int num = 0;
				while (folder == null && num <= 1)
				{
					StoreObjectId storeObjectId;
					lock (this.setting.LockObj)
					{
						if (!this.setting.CachedFolderIds.TryGetValue(base.MailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, out storeObjectId))
						{
							storeObjectId = this.GetFolderId(base.MailboxSession, defaultFolderId, folderName);
							if (storeObjectId == null)
							{
								folder = Folder.Create(base.MailboxSession, defaultFolderId, StoreObjectType.Folder, folderName, CreateMode.OpenIfExists);
								disposeGuard.Add<Folder>(folder);
								folder.Save();
								folder.Load(AuditLogSearchDataProviderBase.FolderProperties);
								storeObjectId = folder.Id.ObjectId;
							}
							this.setting.CachedFolderIds[base.MailboxSession.MailboxOwner.MailboxInfo.MailboxGuid] = storeObjectId;
						}
					}
					if (folder == null)
					{
						try
						{
							folder = Folder.Bind(base.MailboxSession, storeObjectId);
							disposeGuard.Add<Folder>(folder);
						}
						catch (ObjectNotFoundException)
						{
							lock (this.setting.LockObj)
							{
								this.setting.CachedFolderIds.Remove(base.MailboxSession.MailboxOwner.MailboxInfo.MailboxGuid);
							}
							bool flag3 = num >= 1;
							if (flag3)
							{
								throw;
							}
						}
					}
					num++;
				}
				disposeGuard.Success();
			}
			return folder;
		}

		private StoreObjectId GetFolderId(MailboxSession mailboxSession, StoreObjectId rootFolderId, string folderName)
		{
			using (Folder folder = Folder.Bind(mailboxSession, rootFolderId))
			{
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, new PropertyDefinition[]
				{
					FolderSchema.Id,
					StoreObjectSchema.DisplayName
				}))
				{
					QueryFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.DisplayName, folderName);
					if (queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter))
					{
						object[][] rows = queryResult.GetRows(1);
						if (rows.Length > 0)
						{
							VersionedId versionedId = (VersionedId)rows[0][0];
							return versionedId.ObjectId;
						}
					}
				}
			}
			return null;
		}

		internal IEnumerable<VersionedId> FindMessageIds(ObjectId rootId, SortBy sortBy, bool latest)
		{
			SortBy[] sortColumns = (sortBy == null) ? null : new SortBy[]
			{
				sortBy
			};
			ExDateTime momentsAgo = new ExDateTime(ExTimeZone.UtcTimeZone, DateTime.UtcNow.Add(AuditLogSearchDataProviderBase.DelayPeriod));
			QueryFilter queryFilter = this.setting.MessageQueryFilter;
			using (QueryResult queryResult = this.Folder.ItemQuery(ItemQueryType.None, queryFilter, sortColumns, AuditLogSearchDataProviderBase.MessageProperties))
			{
				AuditLogSearchId requestId = rootId as AuditLogSearchId;
				if (requestId != null)
				{
					QueryFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, AuditLogSearchItemSchema.Identity, requestId.Guid);
					SeekReference seekReference = SeekReference.OriginBeginning;
					while (queryResult.SeekToCondition(seekReference, seekFilter))
					{
						seekReference = SeekReference.OriginCurrent;
						foreach (VersionedId messageId in this.ReadMessageIdsFromQueryResult(queryResult, momentsAgo, latest))
						{
							yield return messageId;
						}
					}
				}
				else
				{
					foreach (VersionedId messageId2 in this.ReadMessageIdsFromQueryResult(queryResult, momentsAgo, latest))
					{
						yield return messageId2;
					}
				}
			}
			yield break;
		}

		public override IConfigurable Read<T>(ObjectId identity)
		{
			return this.Find<T>(null, identity, true, null).FirstOrDefault<IConfigurable>();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.Folder != null)
			{
				this.Folder.Dispose();
			}
			base.InternalDispose(disposing);
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			foreach (VersionedId messageId in this.FindMessageIds(rootId, sortBy, true))
			{
				using (AuditLogSearchItemBase requestItem = this.GetItemFromStore(messageId))
				{
					AuditLogSearchBase request = (AuditLogSearchBase)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
					request.Initialize(requestItem);
					yield return (T)((object)request);
				}
			}
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			switch (instance.ObjectState)
			{
			case ObjectState.New:
				this.SaveObjectToStore((AuditLogSearchBase)instance);
				return;
			case ObjectState.Unchanged:
				break;
			case ObjectState.Changed:
				this.SaveRequest((AuditLogSearchBase)instance);
				return;
			case ObjectState.Deleted:
				base.Delete(instance);
				break;
			default:
				return;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AuditLogSearchDataProviderBase>(this);
		}

		private void SaveRequest(AuditLogSearchBase search)
		{
			throw new NotImplementedException();
		}

		protected virtual void SaveObjectToStore(AuditLogSearchBase search)
		{
			throw new NotImplementedException();
		}

		internal virtual AuditLogSearchItemBase GetItemFromStore(VersionedId messageId)
		{
			throw new NotImplementedException();
		}

		internal void DeleteItem(VersionedId messageId)
		{
			base.MailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
			{
				messageId
			});
		}

		internal void DeleteAllItems()
		{
			this.folder.DeleteAllObjects(DeleteItemFlags.SoftDelete);
		}

		private IEnumerable<VersionedId> ReadMessageIdsFromQueryResult(QueryResult queryResult, ExDateTime cutoffDateTime, bool latest)
		{
			object[][] rows = queryResult.GetRows(1000);
			foreach (object[] row in rows)
			{
				VersionedId messageId = PropertyBag.CheckPropertyValue<VersionedId>(ItemSchema.Id, row[0]);
				ExDateTime createTime = PropertyBag.CheckPropertyValue<ExDateTime>(StoreObjectSchema.CreationTime, row[1]);
				if ((latest && createTime > cutoffDateTime) || (!latest && createTime <= cutoffDateTime))
				{
					yield return messageId;
				}
			}
			yield break;
		}

		private const int MaxItemsToQuery = 1000;

		private static readonly TimeSpan DelayPeriod = TimeSpan.FromSeconds(-20.0);

		private static readonly PropertyDefinition[] FolderProperties = new PropertyDefinition[]
		{
			ItemSchema.Id
		};

		private static readonly PropertyDefinition[] MessageProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.CreationTime,
			AuditLogSearchItemSchema.Identity
		};

		private readonly AuditLogSearchDataProviderBase.SearchSetting setting;

		private readonly Folder folder;

		internal class SearchSetting
		{
			internal object LockObj
			{
				get
				{
					return this.lockObj;
				}
			}

			public string FolderName { get; set; }

			public QueryFilter MessageQueryFilter { get; set; }

			public Dictionary<Guid, StoreObjectId> CachedFolderIds { get; set; }

			private readonly object lockObj = new object();
		}
	}
}
