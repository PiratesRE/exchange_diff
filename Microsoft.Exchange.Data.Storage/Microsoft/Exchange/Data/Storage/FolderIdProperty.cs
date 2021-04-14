using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class FolderIdProperty : IdProperty
	{
		public FolderIdProperty() : base("FolderId", typeof(VersionedId), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.EntryId, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ChangeKey, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ContainerClass, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.MapiFolderType, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ExtendedFolderFlagsInternal, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override StoreObjectType GetStoreObjectType(PropertyBag.BasicPropertyStore propertyBag)
		{
			return FolderIdProperty.GetFolderType(propertyBag);
		}

		internal static StoreObjectType GetFolderType(PropertyBag.BasicPropertyStore propertyBag)
		{
			int? num = propertyBag.GetValue(InternalSchema.MapiFolderType) as int?;
			FolderType? folderType = (num != null) ? new FolderType?((FolderType)num.GetValueOrDefault()) : null;
			StoreObjectType storeObjectType = ObjectClass.GetObjectType(propertyBag.GetValue(InternalSchema.ContainerClass) as string);
			bool? flag = propertyBag.GetValue(InternalSchema.IsOutlookSearchFolder) as bool?;
			if (storeObjectType == StoreObjectType.TasksFolder && folderType == FolderType.Search)
			{
				storeObjectType = StoreObjectType.SearchFolder;
			}
			else if (!Folder.IsFolderType(storeObjectType) || storeObjectType == StoreObjectType.Folder)
			{
				if (folderType == FolderType.Search)
				{
					if (flag == true)
					{
						storeObjectType = StoreObjectType.OutlookSearchFolder;
					}
					else
					{
						storeObjectType = StoreObjectType.SearchFolder;
					}
				}
				else
				{
					storeObjectType = StoreObjectType.Folder;
				}
			}
			return storeObjectType;
		}

		protected override bool IsCompatibleId(StoreId id, ICoreObject coreObject)
		{
			return (coreObject == null || coreObject is CoreFolder) && IdConverter.IsFolderId(StoreId.GetStoreObjectId(id));
		}
	}
}
