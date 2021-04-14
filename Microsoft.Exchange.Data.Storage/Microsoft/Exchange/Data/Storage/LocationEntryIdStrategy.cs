using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LocationEntryIdStrategy : EntryIdStrategy
	{
		internal static PropertyBag GetMailboxPropertyBag(DefaultFolderContext context)
		{
			return context.GetMailboxPropertyBag();
		}

		internal static PropertyBag GetInboxOrConfigurationFolderPropertyBag(DefaultFolderContext context)
		{
			return context.GetInboxOrConfigurationFolderPropertyBag();
		}

		internal LocationEntryIdStrategy(StorePropertyDefinition property, LocationEntryIdStrategy.GetLocationPropertyBagDelegate getLocationPropertyBag)
		{
			this.Property = property;
			this.GetLocationPropertyBag = getLocationPropertyBag;
		}

		internal override void GetDependentProperties(object location, IList<StorePropertyDefinition> result)
		{
			if (object.Equals(location, this.GetLocationPropertyBag))
			{
				result.Add(this.Property);
			}
		}

		internal override byte[] GetEntryId(DefaultFolderContext context)
		{
			PropertyBag propertyBag = this.GetLocationPropertyBag(context);
			return propertyBag.TryGetProperty(this.Property) as byte[];
		}

		internal override void SetEntryId(DefaultFolderContext context, byte[] entryId)
		{
			this.SetEntryValueInternal(context, entryId);
		}

		internal override FolderSaveResult UnsetEntryId(DefaultFolderContext context)
		{
			return this.UnsetEntryValueInternal(context);
		}

		private FolderSaveResult UnsetEntryValueInternal(DefaultFolderContext context)
		{
			FolderSaveResult folderSaveResult;
			using (Folder folder = Folder.Bind(context.Session, this.GetFolderId(context, DefaultFolderType.Inbox)))
			{
				folder.Delete(this.Property);
				folderSaveResult = folder.Save();
			}
			if (folderSaveResult.OperationResult != OperationResult.Succeeded)
			{
				return folderSaveResult;
			}
			FolderSaveResult result;
			using (Folder folder2 = Folder.Bind(context.Session, this.GetFolderId(context, DefaultFolderType.Configuration)))
			{
				folder2.Delete(this.Property);
				result = folder2.Save();
			}
			return result;
		}

		protected void SetEntryValueInternal(DefaultFolderContext context, object propertyValue)
		{
			using (Folder folder = Folder.Bind(context.Session, this.GetFolderId(context, DefaultFolderType.Inbox)))
			{
				folder[this.Property] = propertyValue;
				folder.Save();
			}
			using (Folder folder2 = Folder.Bind(context.Session, this.GetFolderId(context, DefaultFolderType.Configuration)))
			{
				folder2[this.Property] = propertyValue;
				folder2.Save();
			}
		}

		private StoreObjectId GetFolderId(DefaultFolderContext context, DefaultFolderType folderType)
		{
			StoreObjectId storeObjectId = context[folderType];
			if (storeObjectId == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ExDefaultFolderNotFound(folderType));
			}
			return storeObjectId;
		}

		protected readonly LocationEntryIdStrategy.GetLocationPropertyBagDelegate GetLocationPropertyBag;

		protected readonly StorePropertyDefinition Property;

		internal delegate PropertyBag GetLocationPropertyBagDelegate(DefaultFolderContext context);
	}
}
