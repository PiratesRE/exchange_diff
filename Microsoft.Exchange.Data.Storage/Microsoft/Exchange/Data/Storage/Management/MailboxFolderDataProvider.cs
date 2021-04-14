using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxFolderDataProvider : MailboxFolderDataProviderBase
	{
		public MailboxFolderDataProvider(ADSessionSettings adSessionSettings, ADUser mailboxOwner, ISecurityAccessToken userToken, string action) : base(adSessionSettings, mailboxOwner, userToken, action)
		{
		}

		public MailboxFolderDataProvider(ADSessionSettings adSessionSettings, ADUser mailboxOwner, string action) : base(adSessionSettings, mailboxOwner, action)
		{
		}

		internal MailboxFolderDataProvider()
		{
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			if (sortBy != null)
			{
				throw new NotSupportedException("sortBy");
			}
			if (rootId != null && !(rootId is MailboxFolderId))
			{
				throw new NotSupportedException("rootId: " + rootId.GetType().FullName);
			}
			if (!typeof(MailboxFolder).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			{
				throw new NotSupportedException("FindPaged: " + typeof(T).FullName);
			}
			if (filter == null)
			{
				filter = MailboxFolderDataProvider.nonHiddenFilter;
			}
			else
			{
				filter = new AndFilter(new QueryFilter[]
				{
					filter,
					MailboxFolderDataProvider.nonHiddenFilter
				});
			}
			Dictionary<StoreObjectId, MapiFolderPath> knownFolderPathsCache = new Dictionary<StoreObjectId, MapiFolderPath>();
			knownFolderPathsCache.Add(base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Root), MapiFolderPath.IpmSubtreeRoot);
			knownFolderPathsCache.Add(base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Configuration), MapiFolderPath.NonIpmSubtreeRoot);
			StoreObjectId xsoRootIdentity;
			MapiFolderPath xsoRootFolderPath;
			if (rootId == null)
			{
				xsoRootIdentity = base.MailboxSession.SafeGetDefaultFolderId(DefaultFolderType.Root);
				xsoRootFolderPath = MapiFolderPath.IpmSubtreeRoot;
			}
			else
			{
				MailboxFolderId mailboxFolderId = (MailboxFolderId)rootId;
				if (mailboxFolderId.StoreObjectIdValue == null)
				{
					StoreObjectId storeObjectId = base.ResolveStoreObjectIdFromFolderPath(mailboxFolderId.MailboxFolderPath);
					if (storeObjectId == null)
					{
						yield break;
					}
					xsoRootIdentity = storeObjectId;
				}
				else
				{
					xsoRootIdentity = mailboxFolderId.StoreObjectIdValue;
				}
				xsoRootFolderPath = mailboxFolderId.MailboxFolderPath;
			}
			MailboxFolder rootFolder = (MailboxFolder)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
			PropertyDefinition[] xsoProperties = rootFolder.Schema.AllDependentXsoProperties;
			Folder xsoFolder = Folder.Bind(base.MailboxSession, xsoRootIdentity, xsoProperties);
			rootFolder.LoadDataFromXso(base.MailboxSession.MailboxOwner.ObjectId, xsoFolder);
			rootFolder.SetDefaultFolderType(base.MailboxSession.IsDefaultFolderType(xsoRootIdentity));
			if (null == xsoRootFolderPath)
			{
				xsoRootFolderPath = MailboxFolderDataProvider.CalculateMailboxFolderPath(base.MailboxSession, rootFolder.InternalFolderIdentity.ObjectId, rootFolder.InternalParentFolderIdentity, rootFolder.Name, knownFolderPathsCache);
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
					MailboxFolder oneFolder = (MailboxFolder)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
					using (Folder oneXsoFolder = Folder.Bind(base.MailboxSession, ((VersionedId)row[0]).ObjectId, xsoProperties))
					{
						oneFolder.LoadDataFromXso(base.MailboxSession.MailboxOwner.ObjectId, oneXsoFolder);
						oneFolder.SetDefaultFolderType(base.MailboxSession.IsDefaultFolderType(oneFolder.InternalFolderIdentity));
						oneFolder.FolderPath = MailboxFolderDataProvider.CalculateMailboxFolderPath(base.MailboxSession, oneFolder.InternalFolderIdentity.ObjectId, oneFolder.InternalParentFolderIdentity, oneFolder.Name, knownFolderPathsCache);
						yield return (T)((object)oneFolder);
					}
				}
			}
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			MailboxFolder mailboxFolder = instance as MailboxFolder;
			if (mailboxFolder == null)
			{
				throw new NotSupportedException("Save: " + instance.GetType().FullName);
			}
			FolderSaveResult folderSaveResult = null;
			switch (mailboxFolder.ObjectState)
			{
			case ObjectState.New:
				try
				{
					using (Folder folder = Folder.Create(base.MailboxSession, mailboxFolder.InternalParentFolderIdentity, ObjectClass.GetObjectType(mailboxFolder.FolderClass), mailboxFolder.Name, CreateMode.CreateNew))
					{
						mailboxFolder.SaveDataToXso(folder, new ReadOnlyCollection<XsoDriverPropertyDefinition>(new List<XsoDriverPropertyDefinition>
						{
							MailboxFolderSchema.Name,
							MailboxFolderSchema.InternalParentFolderIdentity
						}));
						MailboxFolderDataProvider.ValidateXsoObjectAndThrowForError(mailboxFolder.Name, folder, mailboxFolder.Schema);
						folderSaveResult = folder.Save();
					}
					goto IL_FD;
				}
				catch (ObjectExistedException innerException)
				{
					throw new ObjectExistedException(ServerStrings.ErrorFolderAlreadyExists(mailboxFolder.Name), innerException);
				}
				break;
			case ObjectState.Unchanged:
				goto IL_FD;
			case ObjectState.Changed:
				break;
			case ObjectState.Deleted:
				throw new InvalidOperationException(ServerStrings.ExceptionObjectHasBeenDeleted);
			default:
				goto IL_FD;
			}
			throw new NotImplementedException("Save.Changed");
			IL_FD:
			if (folderSaveResult != null && folderSaveResult.OperationResult != OperationResult.Succeeded)
			{
				throw folderSaveResult.ToException(ServerStrings.ErrorFolderSave(instance.Identity.ToString(), folderSaveResult.ToString()));
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxFolderDataProvider>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		internal static void ValidateXsoObjectAndThrowForError(string identity, StoreObject xsoObject, XsoMailboxConfigurationObjectSchema schema)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (xsoObject == null)
			{
				throw new ArgumentNullException("xsoObject");
			}
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			StoreObjectValidationError[] array = xsoObject.Validate();
			if (array.Length != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (StoreObjectValidationError storeObjectValidationError in array)
				{
					PropertyDefinition propertyDefinition = storeObjectValidationError.PropertyDefinition;
					if (propertyDefinition is StorePropertyDefinition)
					{
						PropertyDefinition relatedWrapperProperty = schema.GetRelatedWrapperProperty(propertyDefinition);
						if (relatedWrapperProperty != null)
						{
							propertyDefinition = relatedWrapperProperty;
						}
					}
					stringBuilder.AppendLine();
					stringBuilder.Append("\t");
					stringBuilder.Append(ServerStrings.ErrorXsoObjectPropertyValidationError(propertyDefinition.Name, storeObjectValidationError.ToString()));
				}
				throw new CorruptDataException(ServerStrings.ErrorFolderSave(identity, stringBuilder.ToString()));
			}
		}

		internal static MapiFolderPath CalculateMailboxFolderPath(StoreSession session, StoreObjectId storeFolderId, StoreObjectId parentStoreFolderId, string displayName, Dictionary<StoreObjectId, MapiFolderPath> knownFolderPathsCache)
		{
			if (storeFolderId == null)
			{
				throw new ArgumentNullException("storeFolderId");
			}
			if (parentStoreFolderId == null)
			{
				throw new ArgumentNullException("parentStoreFolderId");
			}
			if (knownFolderPathsCache == null)
			{
				throw new ArgumentNullException("knownFolderPathsCache");
			}
			Stack<string> stack = new Stack<string>();
			Stack<StoreObjectId> stack2 = new Stack<StoreObjectId>();
			MapiFolderPath mapiFolderPath = null;
			while (null == mapiFolderPath)
			{
				if (knownFolderPathsCache.ContainsKey(storeFolderId))
				{
					mapiFolderPath = knownFolderPathsCache[storeFolderId];
				}
				else if (knownFolderPathsCache.ContainsKey(parentStoreFolderId))
				{
					mapiFolderPath = knownFolderPathsCache[parentStoreFolderId];
					stack.Push(displayName);
					stack2.Push(storeFolderId);
				}
				else
				{
					using (Folder folder = Folder.Bind(session, parentStoreFolderId, new PropertyDefinition[]
					{
						FolderSchema.DisplayName,
						StoreObjectSchema.ParentItemId
					}))
					{
						stack.Push(displayName);
						stack2.Push(storeFolderId);
						storeFolderId = parentStoreFolderId;
						parentStoreFolderId = (StoreObjectId)folder[StoreObjectSchema.ParentItemId];
						displayName = (string)folder[FolderSchema.DisplayName];
					}
				}
			}
			MapiFolderPath mapiFolderPath2 = mapiFolderPath;
			while (0 < stack.Count)
			{
				mapiFolderPath2 = MapiFolderPath.GenerateFolderPath(mapiFolderPath2, stack.Pop());
				StoreObjectId key = stack2.Pop();
				if (!knownFolderPathsCache.ContainsKey(key))
				{
					knownFolderPathsCache.Add(key, mapiFolderPath2);
				}
			}
			return mapiFolderPath2;
		}

		private static readonly ComparisonFilter nonHiddenFilter = new ComparisonFilter(ComparisonOperator.NotEqual, FolderSchema.IsHidden, true);
	}
}
