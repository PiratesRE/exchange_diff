using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AnchorDataProvider : DisposeTrackableBase, IAnchorDataProvider, IDisposable
	{
		private AnchorDataProvider(AnchorContext anchorContext, AnchorADProvider activeDirectoryProvider, MailboxSession mailboxSession, AnchorFolder folder, bool ownSession)
		{
			AnchorUtil.ThrowOnNullArgument(anchorContext, "anchorContext");
			AnchorUtil.ThrowOnNullArgument(activeDirectoryProvider, "activeDirectoryProvider");
			AnchorUtil.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			AnchorUtil.ThrowOnNullArgument(folder, "folder");
			this.anchorContext = anchorContext;
			this.activeDirectoryProvider = activeDirectoryProvider;
			this.MailboxSession = mailboxSession;
			this.folder = folder;
			this.ownSession = ownSession;
		}

		public AnchorContext AnchorContext
		{
			get
			{
				return this.anchorContext;
			}
		}

		public string TenantName
		{
			get
			{
				IExchangePrincipal mailboxOwner = this.MailboxSession.MailboxOwner;
				if (mailboxOwner == null || !(mailboxOwner.MailboxInfo.OrganizationId != null))
				{
					return null;
				}
				if (mailboxOwner.MailboxInfo.OrganizationId.ConfigurationUnit != null)
				{
					return mailboxOwner.MailboxInfo.OrganizationId.ConfigurationUnit.ToString();
				}
				IOrganizationIdForEventLog organizationId = mailboxOwner.MailboxInfo.OrganizationId;
				return organizationId.IdForEventLog;
			}
		}

		public string MailboxName
		{
			get
			{
				return string.Format("{0} :: {1}", this.TenantName, this.MailboxSession.MailboxOwner.MailboxInfo.MailboxGuid);
			}
		}

		public IAnchorADProvider ADProvider
		{
			get
			{
				return this.activeDirectoryProvider;
			}
		}

		public Guid MdbGuid
		{
			get
			{
				return this.mailboxSession.MailboxOwner.MailboxInfo.GetDatabaseGuid();
			}
		}

		public MailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
			private set
			{
				this.mailboxSession = value;
			}
		}

		public IAnchorStoreObject Folder
		{
			get
			{
				return this.folder;
			}
		}

		public ADObjectId OwnerId
		{
			get
			{
				return this.MailboxSession.MailboxOwner.ObjectId;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.MailboxSession.MailboxOwner.MailboxInfo.OrganizationId;
			}
		}

		public static AnchorDataProvider CreateProviderForMigrationMailboxFolder(AnchorContext context, AnchorADProvider activeDirectoryProvider, string folderName)
		{
			AnchorUtil.ThrowOnNullArgument(context, "context");
			AnchorUtil.ThrowOnNullArgument(activeDirectoryProvider, "activeDirectoryProvider");
			AnchorUtil.ThrowOnNullArgument(folderName, "folderName");
			return AnchorDataProvider.CreateProviderForMailboxSession(context, activeDirectoryProvider, folderName, new Func<ExchangePrincipal, MailboxSession>(AnchorDataProvider.OpenMailboxSession));
		}

		public IAnchorMessageItem CreateMessage()
		{
			return new AnchorMessageItem(this.anchorContext, MessageItem.CreateAssociated(this.MailboxSession, this.folder.Id));
		}

		public bool MoveMessageItems(StoreObjectId[] itemsToMove, string folderName)
		{
			bool result;
			using (AnchorFolder anchorFolder = AnchorFolder.GetFolder(this.anchorContext, this.MailboxSession, folderName))
			{
				GroupOperationResult groupOperationResult = this.folder.Folder.MoveItems(anchorFolder.Folder.Id, itemsToMove);
				result = (groupOperationResult.OperationResult == OperationResult.Succeeded);
			}
			return result;
		}

		public IAnchorEmailMessageItem CreateEmailMessage()
		{
			return new AnchorEmailMessageItem(this.anchorContext, this.ADProvider, MessageItem.Create(this.MailboxSession, this.MailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts)));
		}

		public void RemoveMessage(StoreObjectId messageId)
		{
			AnchorUtil.ThrowOnNullArgument(messageId, "messageId");
			try
			{
				this.MailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
				{
					messageId
				});
			}
			catch (ObjectNotFoundException exception)
			{
				this.anchorContext.Logger.Log(MigrationEventType.Error, exception, "Encountered an object not found  exception when deleting a migration job cache entry message", new object[0]);
			}
		}

		public IEnumerable<StoreObjectId> FindMessageIds(QueryFilter queryFilter, PropertyDefinition[] properties, SortBy[] sortBy, AnchorRowSelector rowSelector, int? maxCount)
		{
			AnchorUtil.ThrowOnCollectionEmptyArgument(sortBy, "sortBy");
			if (maxCount == null || maxCount.Value > 0)
			{
				if (properties == null)
				{
					properties = new PropertyDefinition[0];
				}
				PropertyDefinition[] columns = new PropertyDefinition[1 + properties.Length];
				columns[0] = ItemSchema.Id;
				Array.Copy(properties, 0, columns, 1, properties.Length);
				using (IQueryResult itemQueryResult = this.folder.Folder.IItemQuery(ItemQueryType.Associated, queryFilter, sortBy, columns))
				{
					foreach (StoreObjectId id in AnchorDataProvider.ProcessQueryRows(columns, itemQueryResult, rowSelector, 0, maxCount))
					{
						yield return id;
					}
				}
			}
			yield break;
		}

		public IAnchorMessageItem FindMessage(StoreObjectId messageId, PropertyDefinition[] properties)
		{
			AnchorUtil.ThrowOnNullArgument(messageId, "messageId");
			AnchorUtil.ThrowOnNullArgument(properties, "properties");
			return new AnchorMessageItem(this.mailboxSession, messageId, properties);
		}

		public IAnchorStoreObject GetFolderByName(string folderName, PropertyDefinition[] properties)
		{
			AnchorUtil.ThrowOnNullArgument(properties, "properties");
			AnchorUtil.AssertOrThrow(this.MailboxSession != null, "Should have a MailboxSession", new object[0]);
			IAnchorStoreObject result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IAnchorStoreObject anchorStoreObject = AnchorFolder.GetFolder(this.anchorContext, this.MailboxSession, folderName);
				disposeGuard.Add<IAnchorStoreObject>(anchorStoreObject);
				anchorStoreObject.Load(properties);
				disposeGuard.Success();
				result = anchorStoreObject;
			}
			return result;
		}

		public IAnchorDataProvider GetProviderForFolder(AnchorContext context, string folderName)
		{
			IAnchorDataProvider result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				AnchorFolder disposable = AnchorFolder.GetFolder(this.anchorContext, this.MailboxSession, folderName);
				disposeGuard.Add<AnchorFolder>(disposable);
				AnchorDataProvider anchorDataProvider = new AnchorDataProvider(context, this.activeDirectoryProvider, this.MailboxSession, disposable, false);
				disposeGuard.Success();
				result = anchorDataProvider;
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.folder != null)
				{
					this.folder.Dispose();
				}
				this.folder = null;
				if (this.ownSession && this.MailboxSession != null)
				{
					this.MailboxSession.Dispose();
				}
				this.MailboxSession = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AnchorDataProvider>(this);
		}

		private static AnchorDataProvider CreateProviderForMailboxSession(AnchorContext context, AnchorADProvider activeDirectoryProvider, string folderName, Func<ExchangePrincipal, MailboxSession> mailboxSessionCreator)
		{
			AnchorUtil.ThrowOnNullArgument(mailboxSessionCreator, "mailboxSessionCreator");
			AnchorDataProvider result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				ExchangePrincipal mailboxOwner = activeDirectoryProvider.GetMailboxOwner(AnchorDataProvider.GetMailboxFilter(context.AnchorCapability));
				MailboxSession disposable = mailboxSessionCreator(mailboxOwner);
				disposeGuard.Add<MailboxSession>(disposable);
				AnchorFolder disposable2 = AnchorFolder.GetFolder(context, disposable, folderName);
				disposeGuard.Add<AnchorFolder>(disposable2);
				AnchorDataProvider anchorDataProvider = new AnchorDataProvider(context, activeDirectoryProvider, disposable, disposable2, true);
				disposeGuard.Success();
				result = anchorDataProvider;
			}
			return result;
		}

		private static QueryFilter GetMailboxFilter(OrganizationCapability anchorCapability)
		{
			return QueryFilter.AndTogether(new QueryFilter[]
			{
				OrganizationMailbox.OrganizationMailboxFilterBase,
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RawCapabilities, anchorCapability)
			});
		}

		private static MailboxSession OpenMailboxSession(ExchangePrincipal mailboxOwner)
		{
			return MailboxSession.OpenAsSystemService(mailboxOwner, CultureInfo.InvariantCulture, "Client=AnchorService;Privilege:OpenAsSystemService");
		}

		private static IEnumerable<StoreObjectId> ProcessQueryRows(PropertyDefinition[] propertyDefinitions, IQueryResult itemQueryResult, AnchorRowSelector rowSelectorPredicate, int idColumnIndex, int? maxCount)
		{
			Dictionary<PropertyDefinition, object> rowData = new Dictionary<PropertyDefinition, object>(propertyDefinitions.Length);
			int matchingRowCount = 0;
			bool mightBeMoreRows = true;
			while (mightBeMoreRows)
			{
				object[][] rows = itemQueryResult.GetRows(100, out mightBeMoreRows);
				if (!mightBeMoreRows)
				{
					break;
				}
				foreach (object[] row in rows)
				{
					rowData.Clear();
					for (int j = 0; j < propertyDefinitions.Length; j++)
					{
						rowData[propertyDefinitions[j]] = row[j];
					}
					switch (rowSelectorPredicate(rowData))
					{
					case AnchorRowSelectorResult.AcceptRow:
						matchingRowCount++;
						yield return ((VersionedId)row[idColumnIndex]).ObjectId;
						if (maxCount != null && matchingRowCount == maxCount)
						{
							mightBeMoreRows = false;
						}
						break;
					case AnchorRowSelectorResult.RejectRowContinueProcessing:
						break;
					case AnchorRowSelectorResult.RejectRowStopProcessing:
						mightBeMoreRows = false;
						break;
					default:
						mightBeMoreRows = false;
						break;
					}
					if (!mightBeMoreRows)
					{
						break;
					}
				}
			}
			yield break;
		}

		public const string ServiceletConnectionString = "Client=AnchorService;Privilege:OpenAsSystemService";

		private const int DefaultBatchSize = 100;

		private readonly bool ownSession;

		private readonly AnchorADProvider activeDirectoryProvider;

		private readonly AnchorContext anchorContext;

		private MailboxSession mailboxSession;

		private AnchorFolder folder;
	}
}
