using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderDataProvider : XsoStoreDataProviderBase
	{
		public PublicFolderSessionCache PublicFolderSessionCache
		{
			get
			{
				return this.publicFolderSessionCache;
			}
		}

		public OrganizationId CurrentOrganizationId
		{
			get
			{
				return this.currentOrganizationId;
			}
		}

		public PublicFolderDataProvider(IConfigurationSession configurationSession, string action, Guid mailboxGuid)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				Util.ThrowOnNullArgument(configurationSession, "configurationSession");
				Util.ThrowOnNullOrEmptyArgument(action, "action");
				this.currentOrganizationId = configurationSession.GetOrgContainer().OrganizationId;
				TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(this.currentOrganizationId);
				if (mailboxGuid == Guid.Empty)
				{
					Organization orgContainer = configurationSession.GetOrgContainer();
					if (orgContainer.DefaultPublicFolderMailbox.HierarchyMailboxGuid != value.GetHierarchyMailboxInformation().HierarchyMailboxGuid)
					{
						TenantPublicFolderConfigurationCache.Instance.RemoveValue(this.currentOrganizationId);
					}
				}
				else if (value.GetLocalMailboxRecipient(mailboxGuid) == null)
				{
					TenantPublicFolderConfigurationCache.Instance.RemoveValue(this.currentOrganizationId);
				}
				this.publicFolderSessionCache = new PublicFolderSessionCache(configurationSession.SessionSettings.CurrentOrganizationId, null, null, CultureInfo.InvariantCulture, string.Format("Client=Management;Action={0}", action), null, null, true);
				this.PublicFolderSession = this.publicFolderSessionCache.GetPublicFolderSession(mailboxGuid);
				disposeGuard.Success();
			}
		}

		public PublicFolderSession PublicFolderSession
		{
			get
			{
				return (PublicFolderSession)base.StoreSession;
			}
			private set
			{
				base.StoreSession = value;
			}
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			PublicFolder publicFolder = instance as PublicFolder;
			if (publicFolder == null)
			{
				throw new NotSupportedException("Save: " + instance.GetType().FullName);
			}
			FolderSaveResult folderSaveResult = null;
			switch (publicFolder.ObjectState)
			{
			case ObjectState.New:
				try
				{
					using (Folder folder = Folder.Create(this.PublicFolderSession, publicFolder.InternalParentFolderIdentity, ObjectClass.GetObjectType(publicFolder.FolderClass), publicFolder.Name, CreateMode.CreateNew))
					{
						publicFolder.SaveDataToXso(folder, new ReadOnlyCollection<XsoDriverPropertyDefinition>(new List<XsoDriverPropertyDefinition>
						{
							PublicFolderSchema.Name,
							MailboxFolderSchema.InternalParentFolderIdentity
						}));
						MailboxFolderDataProvider.ValidateXsoObjectAndThrowForError(publicFolder.Name, folder, publicFolder.Schema);
						folderSaveResult = folder.Save();
						publicFolder.OrganizationId = this.PublicFolderSession.OrganizationId;
					}
					goto IL_157;
				}
				catch (ObjectExistedException innerException)
				{
					throw new ObjectExistedException(ServerStrings.ErrorFolderAlreadyExists(publicFolder.Name), innerException);
				}
				break;
			case ObjectState.Unchanged:
				goto IL_157;
			case ObjectState.Changed:
				break;
			case ObjectState.Deleted:
				goto IL_147;
			default:
				goto IL_157;
			}
			using (Folder folder2 = Folder.Bind(this.PublicFolderSession, publicFolder.InternalFolderIdentity, null))
			{
				publicFolder.SaveDataToXso(folder2, new ReadOnlyCollection<XsoDriverPropertyDefinition>(new List<XsoDriverPropertyDefinition>
				{
					MailboxFolderSchema.InternalParentFolderIdentity
				}));
				MailboxFolderDataProvider.ValidateXsoObjectAndThrowForError(publicFolder.Name, folder2, publicFolder.Schema);
				folderSaveResult = folder2.Save();
				goto IL_157;
			}
			IL_147:
			throw new InvalidOperationException(ServerStrings.ExceptionObjectHasBeenDeleted);
			IL_157:
			if (folderSaveResult != null && folderSaveResult.OperationResult != OperationResult.Succeeded && folderSaveResult.PropertyErrors.Length > 0)
			{
				foreach (PropertyError propertyError in folderSaveResult.PropertyErrors)
				{
					if (propertyError.PropertyErrorCode == PropertyErrorCode.FolderNameConflict)
					{
						throw new ObjectExistedException(ServerStrings.ErrorFolderAlreadyExists(publicFolder.Name));
					}
				}
				throw folderSaveResult.ToException(ServerStrings.ErrorFolderSave(instance.Identity.ToString(), folderSaveResult.ToString()));
			}
		}

		protected override void InternalDelete(ConfigurableObject instance)
		{
			PublicFolder publicFolder = instance as PublicFolder;
			if (publicFolder == null)
			{
				throw new NotSupportedException(ServerStrings.ExceptionIsNotPublicFolder(instance.GetType().FullName));
			}
			if (publicFolder.ObjectState == ObjectState.Deleted)
			{
				throw new InvalidOperationException(ServerStrings.ExceptionObjectHasBeenDeleted);
			}
			AggregateOperationResult aggregateOperationResult = this.PublicFolderSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
			{
				this.PublicFolderSession.IdConverter.GetSessionSpecificId(publicFolder.InternalFolderIdentity.ObjectId)
			});
			if (aggregateOperationResult != null && aggregateOperationResult.OperationResult != OperationResult.Succeeded)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (GroupOperationResult groupOperationResult in aggregateOperationResult.GroupOperationResults)
				{
					if (groupOperationResult.OperationResult != OperationResult.Succeeded && groupOperationResult.Exception != null)
					{
						stringBuilder.AppendLine(groupOperationResult.Exception.ToString());
					}
				}
				throw new StoragePermanentException(ServerStrings.ErrorFailedToDeletePublicFolder(publicFolder.Identity.ToString(), stringBuilder.ToString()));
			}
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			if (sortBy != null)
			{
				throw new NotSupportedException("sortBy");
			}
			if (rootId != null && !(rootId is PublicFolderId))
			{
				throw new NotSupportedException("rootId: " + rootId.GetType().FullName);
			}
			if (!typeof(PublicFolder).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			{
				throw new NotSupportedException("FindPaged: " + typeof(T).FullName);
			}
			if (filter == null)
			{
				filter = PublicFolderDataProvider.nonHiddenFilter;
			}
			else
			{
				filter = new AndFilter(new QueryFilter[]
				{
					filter,
					PublicFolderDataProvider.nonHiddenFilter
				});
			}
			Dictionary<StoreObjectId, MapiFolderPath> knownFolderPathsCache = new Dictionary<StoreObjectId, MapiFolderPath>();
			knownFolderPathsCache.Add(this.PublicFolderSession.GetPublicFolderRootId(), MapiFolderPath.IpmSubtreeRoot);
			knownFolderPathsCache.Add(this.PublicFolderSession.GetIpmSubtreeFolderId(), MapiFolderPath.IpmSubtreeRoot);
			knownFolderPathsCache.Add(this.PublicFolderSession.GetNonIpmSubtreeFolderId(), MapiFolderPath.NonIpmSubtreeRoot);
			StoreObjectId xsoRootIdentity;
			MapiFolderPath xsoRootFolderPath;
			if (rootId == null)
			{
				xsoRootIdentity = this.PublicFolderSession.GetIpmSubtreeFolderId();
				xsoRootFolderPath = MapiFolderPath.IpmSubtreeRoot;
			}
			else
			{
				PublicFolderId publicFolderId = (PublicFolderId)rootId;
				if (publicFolderId.StoreObjectId == null)
				{
					StoreObjectId storeObjectId = this.ResolveStoreObjectIdFromFolderPath(publicFolderId.MapiFolderPath);
					if (storeObjectId == null)
					{
						yield break;
					}
					xsoRootIdentity = storeObjectId;
				}
				else
				{
					xsoRootIdentity = publicFolderId.StoreObjectId;
				}
				xsoRootFolderPath = publicFolderId.MapiFolderPath;
			}
			PublicFolder rootFolder = new PublicFolder();
			PropertyDefinition[] xsoProperties = rootFolder.Schema.AllDependentXsoProperties;
			Folder xsoFolder = Folder.Bind(this.PublicFolderSession, xsoRootIdentity, xsoProperties);
			rootFolder.LoadDataFromXso(this.PublicFolderSession.MailboxPrincipal.ObjectId, xsoFolder);
			rootFolder.SetDefaultFolderType(DefaultFolderType.None);
			rootFolder.OrganizationId = this.PublicFolderSession.OrganizationId;
			StoreObjectId receoverableItemsDeletionFolderId = PublicFolderCOWSession.GetRecoverableItemsDeletionsFolderId(xsoFolder);
			rootFolder.DumpsterEntryId = ((receoverableItemsDeletionFolderId != null) ? receoverableItemsDeletionFolderId.ToHexEntryId() : null);
			if (null == xsoRootFolderPath)
			{
				xsoRootFolderPath = MailboxFolderDataProvider.CalculateMailboxFolderPath(this.PublicFolderSession, rootFolder.InternalFolderIdentity.ObjectId, rootFolder.InternalParentFolderIdentity, rootFolder.Name, knownFolderPathsCache);
			}
			else if (!knownFolderPathsCache.ContainsKey(rootFolder.InternalFolderIdentity.ObjectId))
			{
				knownFolderPathsCache.Add(rootFolder.InternalFolderIdentity.ObjectId, xsoRootFolderPath);
			}
			rootFolder.FolderPath = xsoRootFolderPath;
			if (deepSearch)
			{
				yield return (T)((object)rootFolder);
			}
			QueryResult queryResults = xsoFolder.FolderQuery(deepSearch ? FolderQueryFlags.DeepTraversal : FolderQueryFlags.None, filter, null, new PropertyDefinition[]
			{
				FolderSchema.Id
			});
			for (;;)
			{
				object[][] folderRows = queryResults.GetRows((pageSize == 0) ? 1000 : pageSize);
				if (folderRows.Length <= 0)
				{
					break;
				}
				foreach (object[] row in folderRows)
				{
					PublicFolder onePublicFolder = new PublicFolder();
					using (Folder oneXsoFolder = Folder.Bind(this.PublicFolderSession, ((VersionedId)row[0]).ObjectId, xsoProperties))
					{
						onePublicFolder.LoadDataFromXso(this.PublicFolderSession.MailboxPrincipal.ObjectId, oneXsoFolder);
						onePublicFolder.SetDefaultFolderType(DefaultFolderType.None);
						onePublicFolder.OrganizationId = this.PublicFolderSession.OrganizationId;
						receoverableItemsDeletionFolderId = PublicFolderCOWSession.GetRecoverableItemsDeletionsFolderId(oneXsoFolder);
						onePublicFolder.DumpsterEntryId = ((receoverableItemsDeletionFolderId != null) ? receoverableItemsDeletionFolderId.ToHexEntryId() : null);
						onePublicFolder.FolderPath = MailboxFolderDataProvider.CalculateMailboxFolderPath(this.PublicFolderSession, onePublicFolder.InternalFolderIdentity.ObjectId, onePublicFolder.InternalParentFolderIdentity, onePublicFolder.Name, knownFolderPathsCache);
						yield return (T)((object)onePublicFolder);
					}
				}
			}
			yield break;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderDataProvider>(this);
		}

		public StoreObjectId ResolveStoreObjectIdFromFolderPath(MapiFolderPath folderPath)
		{
			Util.ThrowOnNullArgument(folderPath, "folderPath");
			StoreObjectId storeObjectId;
			if (folderPath.IsNonIpmPath)
			{
				storeObjectId = this.PublicFolderSession.GetNonIpmSubtreeFolderId();
			}
			else
			{
				storeObjectId = this.PublicFolderSession.GetIpmSubtreeFolderId();
			}
			if (folderPath.Depth <= 0)
			{
				return storeObjectId;
			}
			foreach (string text in folderPath)
			{
				QueryFilter seekFilter = new TextFilter(FolderSchema.DisplayName, text, MatchOptions.FullString, MatchFlags.IgnoreCase);
				using (Folder folder = Folder.Bind(this.PublicFolderSession, storeObjectId, PublicFolderDataProvider.FolderQueryReturnColumns))
				{
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, PublicFolderDataProvider.FolderQuerySorts, PublicFolderDataProvider.FolderQueryReturnColumns))
					{
						if (!queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter, SeekToConditionFlags.AllowExtendedFilters))
						{
							return null;
						}
						object[][] rows = queryResult.GetRows(1);
						storeObjectId = ((VersionedId)rows[0][0]).ObjectId;
					}
				}
			}
			return storeObjectId;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.publicFolderSessionCache != null)
				{
					this.publicFolderSessionCache.Dispose();
					this.publicFolderSessionCache = null;
				}
				this.PublicFolderSession = null;
			}
			base.InternalDispose(disposing);
		}

		private static readonly ComparisonFilter nonHiddenFilter = new ComparisonFilter(ComparisonOperator.NotEqual, FolderSchema.IsHidden, true);

		private static readonly PropertyDefinition[] FolderQueryReturnColumns = new PropertyDefinition[]
		{
			FolderSchema.Id,
			FolderSchema.DisplayName
		};

		private static readonly SortBy[] FolderQuerySorts = new SortBy[]
		{
			new SortBy(FolderSchema.DisplayName, SortOrder.Ascending)
		};

		private OrganizationId currentOrganizationId;

		private PublicFolderSessionCache publicFolderSessionCache;

		private enum FolderQueryReturnColumnIndex
		{
			Id,
			DisplayName
		}
	}
}
