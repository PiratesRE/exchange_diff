using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DefaultFolderCreator
	{
		internal DefaultFolderType DefaultContainer
		{
			get
			{
				return this.defaultContainer;
			}
		}

		internal DefaultFolderCreator(DefaultFolderType container, StoreObjectType storeObjectType, bool bindByNameIfAlreadyExists = true)
		{
			this.defaultContainer = container;
			this.storeObjectType = storeObjectType;
			this.bindByNameIfAlreadyExists = bindByNameIfAlreadyExists;
		}

		internal static Folder BindToSubfolderByName(StoreSession session, StoreObjectId containerId, string folderName, params PropertyDefinition[] propsToReturn)
		{
			Folder result;
			using (Folder folder = Folder.Bind(session, containerId))
			{
				MapiFolder mapiFolder = null;
				Folder folder2 = null;
				bool flag = false;
				try
				{
					object thisObject = null;
					bool flag2 = false;
					try
					{
						if (session != null)
						{
							session.BeginMapiCall();
							session.BeginServerHealthCall();
							flag2 = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						mapiFolder = folder.MapiFolder.OpenSubFolderByName(folderName);
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenFolder, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("DefaultFolderCreator::BindToSubfolderByName. Unable to open folder by name.", new object[0]),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenFolder, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("DefaultFolderCreator::BindToSubfolderByName. Unable to open folder by name.", new object[0]),
							ex2
						});
					}
					finally
					{
						try
						{
							if (session != null)
							{
								session.EndMapiCall();
								if (flag2)
								{
									session.EndServerHealthCall();
								}
							}
						}
						finally
						{
							if (StorageGlobals.MapiTestHookAfterCall != null)
							{
								StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
							}
						}
					}
					using (MapiPropertyBag mapiPropertyBag = new MapiPropertyBag(session, mapiFolder))
					{
						byte[] entryId = (byte[])mapiPropertyBag.GetProperties(new NativeStorePropertyDefinition[]
						{
							InternalSchema.EntryId
						})[0];
						StoreObjectId folderObjectId = StoreObjectId.FromProviderSpecificId(entryId);
						folder2 = Folder.InternalBind<Folder>(session, mapiFolder, folderObjectId, null, propsToReturn);
						mapiPropertyBag.DetachMapiProp();
					}
					flag = true;
					result = folder2;
				}
				finally
				{
					if (!flag)
					{
						Util.DisposeIfPresent(folder2);
						Util.DisposeIfPresent(mapiFolder);
					}
				}
			}
			return result;
		}

		internal static PropertyError UpdateElcRootFolderName(DefaultFolderContext context, string newName)
		{
			PropertyError result = null;
			StoreObjectId storeObjectId = context[DefaultFolderType.ElcRoot];
			if (storeObjectId != null)
			{
				using (MapiPropertyBag mapiPropertyBag = MapiPropertyBag.CreateMapiPropertyBag(context.Session, storeObjectId))
				{
					PropertyDefinition[] propertyDefinitions = new PropertyDefinition[]
					{
						FolderSchema.DisplayName
					};
					PropertyError[] array = mapiPropertyBag.SetProperties(propertyDefinitions, new object[]
					{
						newName
					});
					if (array.Length > 0)
					{
						result = array[0];
					}
					mapiPropertyBag.SaveChanges(false);
					return result;
				}
			}
			throw new ObjectNotFoundException(ServerStrings.ExDefaultFolderNotFound(DefaultFolderType.ElcRoot));
		}

		internal virtual Folder Create(DefaultFolderContext context, string folderName, out bool hasCreatedNew)
		{
			return this.Create(context, folderName, context[this.defaultContainer], out hasCreatedNew);
		}

		internal virtual Folder Create(DefaultFolderContext context, string folderName, StoreObjectId parentId, out bool hasCreatedNew)
		{
			return this.CreateInternal(context, folderName, parentId, out hasCreatedNew);
		}

		internal virtual AggregateOperationResult Delete(DefaultFolderContext context, DeleteItemFlags deleteItemFlags, StoreObjectId id)
		{
			if (this.storeObjectType == StoreObjectType.OutlookSearchFolder)
			{
				return OutlookSearchFolder.DeleteOutlookSearchFolder(deleteItemFlags, context.Session, id);
			}
			return context.Session.Delete(deleteItemFlags, new StoreId[]
			{
				id
			});
		}

		protected Folder CreateInternal(DefaultFolderContext context, string displayName, StoreObjectId parentFolderId, out bool hasCreatedNew)
		{
			hasCreatedNew = false;
			bool flag = false;
			Folder folder = null;
			try
			{
				folder = this.CreateNewFolder(context, displayName, parentFolderId);
				FolderSaveResult folderSaveResult = folder.Save();
				if (folderSaveResult.OperationResult != OperationResult.Succeeded)
				{
					return null;
				}
				folder.Load(null);
				hasCreatedNew = true;
				flag = true;
			}
			catch (ObjectExistedException)
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceDebug<DefaultFolderCreator, string>((long)this.GetHashCode(), "DefaultFolderCreator::CreateInternal. We found the folder has existed. strategy = {0}, displayName = {1}.", this, displayName);
				if (folder != null)
				{
					folder.Dispose();
					folder = null;
				}
				if (this.bindByNameIfAlreadyExists)
				{
					folder = DefaultFolderCreator.BindToSubfolderByName(context.Session, parentFolderId, displayName, new PropertyDefinition[0]);
					flag = true;
				}
			}
			finally
			{
				if (!flag && folder != null)
				{
					folder.Dispose();
					folder = null;
				}
			}
			return folder;
		}

		private Folder CreateNewFolder(DefaultFolderContext context, string displayName, StoreObjectId parentFolderObjectId)
		{
			Folder result;
			if (this.storeObjectType == StoreObjectType.Folder || this.storeObjectType == StoreObjectType.ContactsFolder)
			{
				result = Folder.Create(context.Session, parentFolderObjectId, this.storeObjectType, displayName, CreateMode.CreateNew);
			}
			else if (this.storeObjectType == StoreObjectType.OutlookSearchFolder)
			{
				result = OutlookSearchFolder.Create(context.Session, displayName);
			}
			else
			{
				if (this.storeObjectType != StoreObjectType.SearchFolder)
				{
					throw new NotSupportedException(string.Format("The type of folder cannot be created. type = {0}.", this.storeObjectType));
				}
				result = SearchFolder.Create(context.Session, parentFolderObjectId, displayName, CreateMode.CreateNew);
			}
			return result;
		}

		internal static DefaultFolderCreator NoCreator = new FolderNoCreator();

		private DefaultFolderType defaultContainer;

		private readonly bool bindByNameIfAlreadyExists;

		private StoreObjectType storeObjectType;
	}
}
