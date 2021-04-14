using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationDataProvider : DisposeTrackableBase, IMigrationDataProvider, IDisposable
	{
		private MigrationDataProvider(MigrationADProvider activeDirectoryProvider, MailboxSession mailboxSession, MigrationFolder folder, bool ownSession)
		{
			MigrationUtil.ThrowOnNullArgument(activeDirectoryProvider, "activeDirectoryProvider");
			MigrationUtil.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			MigrationUtil.ThrowOnNullArgument(folder, "folder");
			this.activeDirectoryProvider = activeDirectoryProvider;
			this.MailboxSession = mailboxSession;
			this.migrationFolder = folder;
			this.ownSession = ownSession;
			this.runspaceProxy = new Lazy<IMigrationRunspaceProxy>(() => MigrationServiceFactory.Instance.CreateRunspaceForDatacenterAdmin(this.OrganizationId));
			this.orgConfigContext = new OrganizationSettingsContext(this.OrganizationId, null).Activate();
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
				if (mailboxOwner.MailboxInfo.OrganizationId.OrganizationalUnit != null)
				{
					return mailboxOwner.MailboxInfo.OrganizationId.OrganizationalUnit.Name;
				}
				IOrganizationIdForEventLog organizationId = mailboxOwner.MailboxInfo.OrganizationId;
				return organizationId.IdForEventLog;
			}
		}

		public string MailboxName
		{
			get
			{
				return string.Format("{0} :: {1}", this.TenantName, this.MailboxSession.MailboxOwnerLegacyDN);
			}
		}

		public IMigrationADProvider ADProvider
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
				return this.mailboxSession.MdbGuid;
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

		public IMigrationStoreObject Folder
		{
			get
			{
				return this.migrationFolder;
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

		public IMigrationRunspaceProxy RunspaceProxy
		{
			get
			{
				return this.runspaceProxy.Value;
			}
		}

		public static MigrationDataProvider CreateProviderForMigrationMailbox(OrganizationId orgId, string migrationMailboxLegDn)
		{
			return MigrationDataProvider.CreateProviderForMigrationMailbox(TenantPartitionHint.FromOrganizationId(orgId), migrationMailboxLegDn);
		}

		public static MigrationDataProvider CreateProviderForMigrationMailbox(TenantPartitionHint tenantPartitionHint, string migrationMailboxLegacyDN)
		{
			MigrationUtil.ThrowOnNullArgument(tenantPartitionHint, "tenantPartitionHint");
			MigrationUtil.ThrowOnNullOrEmptyArgument(migrationMailboxLegacyDN, "migrationMailboxLegacyDN");
			return MigrationDataProvider.CreateProviderForMailboxSession(new MigrationADProvider(tenantPartitionHint), MigrationFolderName.SyncMigration, (MigrationADProvider provider) => MigrationDataProvider.OpenLocalMigrationMailboxSession(provider, migrationMailboxLegacyDN));
		}

		public static MigrationDataProvider CreateProviderForMigrationMailbox(string action, IRecipientSession recipientSession, ADUser partitionMailbox)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(action, "action");
			MigrationUtil.ThrowOnNullArgument(recipientSession, "recipientSession");
			MigrationUtil.ThrowOnNullArgument(partitionMailbox, "partitionMailbox");
			return MigrationDataProvider.CreateProviderForMailboxSession(new MigrationADProvider(recipientSession), MigrationFolderName.SyncMigration, (MigrationADProvider provider) => MigrationDataProvider.OpenMigrationMailboxSession(provider, action, partitionMailbox));
		}

		public static MigrationDataProvider CreateProviderForReportMailbox(string action, IRecipientSession recipientSession, ADUser partitionMailbox)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(action, "action");
			MigrationUtil.ThrowOnNullArgument(recipientSession, "recipientSession");
			MigrationUtil.ThrowOnNullArgument(partitionMailbox, "partitionMailbox");
			return MigrationDataProvider.CreateProviderForMailboxSession(new MigrationADProvider(recipientSession), MigrationFolderName.SyncMigrationReports, (MigrationADProvider provider) => MigrationDataProvider.OpenMigrationMailboxSession(provider, action, partitionMailbox));
		}

		public static MigrationDataProvider CreateProviderForSystemMailbox(Guid mdbGuid)
		{
			MigrationUtil.ThrowOnGuidEmptyArgument(mdbGuid, "mdbGuid");
			return MigrationDataProvider.CreateProviderForMailboxSession(MigrationADProvider.GetRootOrgADProvider(), MigrationFolderName.SyncMigration, (MigrationADProvider provider) => MigrationDataProvider.OpenLocalSystemMailboxSession(provider, mdbGuid));
		}

		public IMigrationMessageItem CreateMessage()
		{
			return new MigrationMessageItem(MessageItem.CreateAssociated(this.MailboxSession, this.migrationFolder.Id));
		}

		public bool MoveMessageItems(StoreObjectId[] itemsToMove, MigrationFolderName folderName)
		{
			bool result;
			using (MigrationFolder folder = MigrationFolder.GetFolder(this.MailboxSession, folderName))
			{
				GroupOperationResult groupOperationResult = this.migrationFolder.Folder.MoveItems(folder.Folder.Id, itemsToMove);
				result = (groupOperationResult.OperationResult == OperationResult.Succeeded);
			}
			return result;
		}

		public IMigrationEmailMessageItem CreateEmailMessage()
		{
			return new MigrationEmailMessageItem(this, MessageItem.Create(this.MailboxSession, this.MailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts)));
		}

		public void RemoveMessage(StoreObjectId messageId)
		{
			MigrationUtil.ThrowOnNullArgument(messageId, "messageId");
			try
			{
				this.MailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
				{
					messageId
				});
			}
			catch (ObjectNotFoundException exception)
			{
				MigrationLogger.Log(MigrationEventType.Error, exception, "Encountered an object not found  exception when deleting a migration job cache entry message", new object[0]);
			}
		}

		public int CountMessages(QueryFilter filter, SortBy[] sortBy)
		{
			return MigrationUtil.RunTimedOperation<int>(delegate()
			{
				int estimatedRowCount;
				using (QueryResult queryResult = this.migrationFolder.Folder.ItemQuery(ItemQueryType.Associated, filter, sortBy, MigrationHelper.ItemIdProperties))
				{
					estimatedRowCount = queryResult.EstimatedRowCount;
				}
				return estimatedRowCount;
			}, filter);
		}

		public object[] QueryRow(QueryFilter filter, SortBy[] sortBy, PropertyDefinition[] propertyDefinitions)
		{
			using (IEnumerator<object[]> enumerator = this.QueryRows(filter, sortBy, propertyDefinitions, 1).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return null;
		}

		public IEnumerable<object[]> QueryRows(QueryFilter filter, SortBy[] sortBy, PropertyDefinition[] propertyDefinitions, int pageSize)
		{
			if (pageSize == 0)
			{
				pageSize = 100;
			}
			QueryResult itemQueryResult = this.migrationFolder.Folder.ItemQuery(ItemQueryType.Associated, filter, sortBy, propertyDefinitions);
			for (;;)
			{
				object[][] rows = itemQueryResult.GetRows(pageSize);
				if (rows == null || rows.Length == 0)
				{
					break;
				}
				foreach (object[] row in rows)
				{
					yield return row;
				}
			}
			yield break;
			yield break;
		}

		public IEnumerable<StoreObjectId> FindMessageIds(QueryFilter filter, PropertyDefinition[] properties, SortBy[] sortBy, MigrationRowSelector rowSelectorPredicate, int? maxCount)
		{
			MigrationUtil.ThrowOnCollectionEmptyArgument(sortBy, "sortBy");
			if (maxCount == null || maxCount.Value > 0)
			{
				if (properties == null)
				{
					properties = new PropertyDefinition[0];
				}
				PropertyDefinition[] columns = new PropertyDefinition[1 + properties.Length];
				columns[0] = ItemSchema.Id;
				Array.Copy(properties, 0, columns, 1, properties.Length);
				using (QueryResult itemQueryResult = this.migrationFolder.Folder.ItemQuery(ItemQueryType.Associated, filter, sortBy, columns))
				{
					foreach (StoreObjectId id in MigrationDataProvider.ProcessQueryRows(columns, itemQueryResult, rowSelectorPredicate, 0, maxCount))
					{
						yield return id;
					}
				}
			}
			yield break;
		}

		public IEnumerable<StoreObjectId> FindMessageIds(MigrationEqualityFilter primaryFilter, PropertyDefinition[] filterColumns, SortBy[] additionalSorts, MigrationRowSelector rowSelectorPredicate, int? maxCount)
		{
			if (maxCount == null || maxCount.Value > 0)
			{
				MigrationUtil.ThrowOnNullArgument(primaryFilter, "primaryFilter");
				if (filterColumns == null)
				{
					filterColumns = new PropertyDefinition[0];
				}
				if (additionalSorts == null)
				{
					additionalSorts = new SortBy[0];
				}
				PropertyDefinition[] propertyDefinitions = new PropertyDefinition[2 + filterColumns.Length];
				propertyDefinitions[0] = primaryFilter.Property;
				propertyDefinitions[1] = ItemSchema.Id;
				Array.Copy(filterColumns, 0, propertyDefinitions, 2, filterColumns.Length);
				SortBy[] sortBy = new SortBy[1 + additionalSorts.Length];
				sortBy[0] = new SortBy(primaryFilter.Property, SortOrder.Ascending);
				Array.Copy(additionalSorts, 0, sortBy, 1, additionalSorts.Length);
				using (QueryResult itemQueryResult = this.migrationFolder.Folder.ItemQuery(ItemQueryType.Associated, null, sortBy, propertyDefinitions))
				{
					SeekReference reference = SeekReference.OriginBeginning;
					if (!MigrationUtil.RunTimedOperation<bool>(() => itemQueryResult.SeekToCondition(reference, primaryFilter.Filter, SeekToConditionFlags.AllowExtendedFilters), primaryFilter.Filter))
					{
						yield break;
					}
					foreach (StoreObjectId id in MigrationDataProvider.ProcessQueryRows(propertyDefinitions, itemQueryResult, rowSelectorPredicate, 1, maxCount))
					{
						yield return id;
					}
				}
			}
			yield break;
		}

		public IMigrationMessageItem FindMessage(StoreObjectId messageId, PropertyDefinition[] properties)
		{
			MigrationUtil.ThrowOnNullArgument(messageId, "messageId");
			MigrationUtil.ThrowOnNullArgument(properties, "properties");
			return new MigrationMessageItem(this, messageId, properties);
		}

		public IMigrationStoreObject GetRootFolder(PropertyDefinition[] properties)
		{
			MigrationUtil.ThrowOnNullArgument(properties, "properties");
			MigrationUtil.AssertOrThrow(this.MailboxSession != null, "Should have a MailboxSession", new object[0]);
			IMigrationStoreObject result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IMigrationStoreObject folder = MigrationFolder.GetFolder(this.MailboxSession, MigrationFolderName.SyncMigration);
				disposeGuard.Add<IMigrationStoreObject>(folder);
				folder.Load(properties);
				disposeGuard.Success();
				result = folder;
			}
			return result;
		}

		public IMigrationDataProvider GetProviderForFolder(MigrationFolderName folderName)
		{
			IMigrationDataProvider result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MigrationFolder folder = MigrationFolder.GetFolder(this.MailboxSession, folderName);
				disposeGuard.Add<MigrationFolder>(folder);
				MigrationDataProvider migrationDataProvider = new MigrationDataProvider(this.activeDirectoryProvider, this.MailboxSession, folder, false);
				disposeGuard.Success();
				result = migrationDataProvider;
			}
			return result;
		}

		public Uri GetEcpUrl()
		{
			return MigrationADProvider.GetEcpUrl(this.MailboxSession.MailboxOwner);
		}

		public void FlushReport(ReportData reportData)
		{
			MigrationUtil.AssertOrThrow(reportData != null, "Cannot flush a null report data.", new object[0]);
			if (!reportData.HasNewEntries)
			{
				return;
			}
			CommonUtils.CatchKnownExceptions(delegate
			{
				reportData.Flush(this.MailboxSession.Mailbox.MapiStore);
			}, delegate(Exception failure)
			{
				MigrationApplication.NotifyOfIgnoredException(failure, "Flushing report data: ");
			});
		}

		public void LoadReport(ReportData reportData)
		{
			MigrationUtil.AssertOrThrow(reportData != null, "Cannot load into a null report data.", new object[0]);
			reportData.Load(this.MailboxSession.Mailbox.MapiStore);
		}

		public void DeleteReport(ReportData reportData)
		{
			MigrationUtil.AssertOrThrow(reportData != null, "Cannot delete using a null report data.", new object[0]);
			CommonUtils.CatchKnownExceptions(delegate
			{
				reportData.Delete(this.MailboxSession.Mailbox.MapiStore);
			}, delegate(Exception failure)
			{
				MigrationApplication.NotifyOfIgnoredException(failure, "Deleting report: ");
			});
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.orgConfigContext != null)
				{
					this.orgConfigContext.Dispose();
					this.orgConfigContext = null;
				}
				if (this.migrationFolder != null)
				{
					this.migrationFolder.Dispose();
				}
				this.migrationFolder = null;
				if (this.ownSession && this.MailboxSession != null)
				{
					this.MailboxSession.Dispose();
				}
				this.MailboxSession = null;
				if (this.runspaceProxy != null && this.runspaceProxy.IsValueCreated && this.runspaceProxy.Value != null)
				{
					this.runspaceProxy.Value.Dispose();
				}
				this.runspaceProxy = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationDataProvider>(this);
		}

		private static MigrationDataProvider CreateProviderForMailboxSession(MigrationADProvider activeDirectoryProvider, MigrationFolderName folderName, Func<MigrationADProvider, MailboxSession> mailboxSessionCreator)
		{
			MigrationUtil.ThrowOnNullArgument(mailboxSessionCreator, "mailboxSessionCreator");
			MigrationDataProvider result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MailboxSession disposable = mailboxSessionCreator(activeDirectoryProvider);
				disposeGuard.Add<MailboxSession>(disposable);
				MigrationFolder folder = MigrationFolder.GetFolder(disposable, folderName);
				disposeGuard.Add<MigrationFolder>(folder);
				MigrationDataProvider migrationDataProvider = new MigrationDataProvider(activeDirectoryProvider, disposable, folder, true);
				disposeGuard.Success();
				result = migrationDataProvider;
			}
			return result;
		}

		private static MailboxSession OpenLocalSystemMailboxSession(MigrationADProvider activeDirectoryProvider, Guid mdbGuid)
		{
			MigrationUtil.ThrowOnNullArgument(activeDirectoryProvider, "activeDirectoryProvider");
			MigrationUtil.ThrowOnGuidEmptyArgument(mdbGuid, "mdbGuid");
			ExchangePrincipal localSystemMailboxOwner = activeDirectoryProvider.GetLocalSystemMailboxOwner(mdbGuid);
			return MigrationDataProvider.OpenLocalMigrationMailboxSession(activeDirectoryProvider, localSystemMailboxOwner);
		}

		private static MailboxSession OpenLocalMigrationMailboxSession(MigrationADProvider activeDirectoryProvider, string migrationMailboxLegacyDN)
		{
			MigrationUtil.ThrowOnNullArgument(activeDirectoryProvider, "activeDirectoryProvider");
			MigrationUtil.ThrowOnNullOrEmptyArgument(migrationMailboxLegacyDN, "migrationMailboxLegacyDN");
			ExchangePrincipal localMigrationMailboxOwner = activeDirectoryProvider.GetLocalMigrationMailboxOwner(migrationMailboxLegacyDN);
			return MigrationDataProvider.OpenLocalMigrationMailboxSession(activeDirectoryProvider, localMigrationMailboxOwner);
		}

		private static MailboxSession OpenMigrationMailboxSession(MigrationADProvider activeDirectoryProvider, string action, ADUser partitionMailbox)
		{
			MigrationUtil.ThrowOnNullArgument(activeDirectoryProvider, "activeDirectoryProvider");
			MigrationUtil.ThrowOnNullOrEmptyArgument(action, "action");
			MigrationUtil.ThrowOnNullArgument(partitionMailbox, "partitionMailbox");
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(activeDirectoryProvider.RecipientSession.SessionSettings, partitionMailbox, RemotingOptions.AllowCrossSite);
			string connectionDescription = string.Format("Client=Management;Privilege:OpenAsSystemService;Action={0}", action);
			return MigrationDataProvider.OpenMigrationMailboxSession(mailboxOwner, connectionDescription);
		}

		private static MailboxSession OpenMigrationMailboxSession(ExchangePrincipal mailboxOwner, string connectionDescription)
		{
			MigrationUtil.ThrowOnNullArgument(mailboxOwner, "mailboxOwner");
			MigrationUtil.ThrowOnNullArgument(connectionDescription, "connectionDescription");
			MailboxSession result;
			try
			{
				result = MailboxSession.OpenAsSystemService(mailboxOwner, CultureInfo.InvariantCulture, connectionDescription);
			}
			catch (DatabaseNotFoundException innerException)
			{
				throw new MigrationTransientException(Strings.MigrationTempMissingDatabase, innerException);
			}
			catch (StoragePermanentException ex)
			{
				if (ex.InnerException is MapiExceptionDuplicateObject || ex.InnerException is MapiExceptionMailboxInTransit)
				{
					throw new MigrationTransientException(Strings.MigrationTempMissingMigrationMailbox, ex);
				}
				throw;
			}
			return result;
		}

		private static MailboxSession OpenLocalMigrationMailboxSession(MigrationADProvider activeDirectoryProvider, ExchangePrincipal mailboxOwner)
		{
			MigrationUtil.ThrowOnNullArgument(activeDirectoryProvider, "activeDirectoryProvider");
			MigrationUtil.ThrowOnNullArgument(mailboxOwner, "mailboxOwner");
			activeDirectoryProvider.EnsureLocalMailbox(mailboxOwner, false);
			MailboxSession result;
			try
			{
				result = MigrationDataProvider.OpenMigrationMailboxSession(mailboxOwner, "Client=MSExchangeSimpleMigration;Privilege:OpenAsSystemService");
			}
			catch (ConnectionFailedPermanentException ex)
			{
				if (ex is WrongServerException || ex is MailboxCrossSiteFailoverException)
				{
					throw new ConnectionFailedTransientException(ServerStrings.PrincipalFromDifferentSite, ex);
				}
				throw;
			}
			return result;
		}

		private static IEnumerable<StoreObjectId> ProcessQueryRows(PropertyDefinition[] propertyDefinitions, QueryResult itemQueryResult, MigrationRowSelector rowSelectorPredicate, int idColumnIndex, int? maxCount)
		{
			Dictionary<PropertyDefinition, object> rowData = new Dictionary<PropertyDefinition, object>(propertyDefinitions.Length);
			int matchingRowCount = 0;
			bool searchFinished = false;
			while (!searchFinished)
			{
				object[][] rows = itemQueryResult.GetRows(100);
				if (rows.Length == 0)
				{
					searchFinished = true;
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
					case MigrationRowSelectorResult.AcceptRow:
						matchingRowCount++;
						yield return ((VersionedId)row[idColumnIndex]).ObjectId;
						if (maxCount != null && matchingRowCount == maxCount)
						{
							searchFinished = true;
						}
						break;
					case MigrationRowSelectorResult.RejectRowStopProcessing:
						searchFinished = true;
						break;
					}
					if (searchFinished)
					{
						break;
					}
				}
			}
			yield break;
		}

		public const string ServiceletConnectionString = "Client=MSExchangeSimpleMigration;Privilege:OpenAsSystemService";

		private const int DefaultBatchSize = 100;

		private const string ManagementConnectionString = "Client=Management;Privilege:OpenAsSystemService;Action={0}";

		private MigrationADProvider activeDirectoryProvider;

		private MailboxSession mailboxSession;

		private MigrationFolder migrationFolder;

		private bool ownSession;

		private Lazy<IMigrationRunspaceProxy> runspaceProxy;

		private IDisposable orgConfigContext;
	}
}
