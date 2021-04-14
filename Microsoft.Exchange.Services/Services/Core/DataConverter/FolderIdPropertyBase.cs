using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class FolderIdPropertyBase : ItemIdPropertyBase
	{
		protected FolderIdPropertyBase(CommandContext commandContext) : base(commandContext)
		{
		}

		public static ConcatenatedIdAndChangeKey GetConcatenatedPublicFolderIdFromStoreId(StoreId storeId)
		{
			StoreObjectId sourceFolderId;
			byte[] array;
			if (storeId is VersionedId)
			{
				sourceFolderId = ((VersionedId)storeId).ObjectId;
				array = ((VersionedId)storeId).ChangeKeyAsByteArray();
			}
			else
			{
				sourceFolderId = (StoreObjectId)storeId;
				array = null;
			}
			StoreObjectId storeObjectId = StoreObjectId.ToNormalizedPublicFolderId(sourceFolderId);
			ConcatenatedIdAndChangeKey concatenatedId;
			if (array != null)
			{
				VersionedId storeId2 = new VersionedId(storeObjectId, array);
				concatenatedId = IdConverter.GetConcatenatedId(storeId2, null, null, null);
			}
			else
			{
				concatenatedId = IdConverter.GetConcatenatedId(storeObjectId, null, null, null);
			}
			return concatenatedId;
		}

		internal override ServiceObjectId CreateServiceObjectId(string id, string changeKey)
		{
			return new FolderId
			{
				Id = id,
				ChangeKey = changeKey
			};
		}

		internal override Array CreateServiceObjectidArray(List<ConcatenatedIdAndChangeKey> ids)
		{
			FolderId[] array = new FolderId[ids.Count];
			for (int i = 0; i < ids.Count; i++)
			{
				array[i] = new FolderId
				{
					Id = ids[i].Id,
					ChangeKey = ids[i].ChangeKey
				};
			}
			return array;
		}

		protected bool TryCheckAndConvertToPublicFolderIdFromStoreObject(CommandOptions convertFolderIdFlag)
		{
			return this.TryCheckAndConvertToPublicFolderIdAndSet(convertFolderIdFlag, null, ToServiceObjectFolderIdConvertSource.StoreObject);
		}

		protected bool TryCheckAndConvertToPublicFolderIdFromPropertyBag(CommandOptions convertFolderIdFlag, PropertyDefinition propertyBagKey)
		{
			return this.TryCheckAndConvertToPublicFolderIdAndSet(convertFolderIdFlag, propertyBagKey, ToServiceObjectFolderIdConvertSource.PropertyBag);
		}

		private bool TryCheckAndConvertToPublicFolderIdAndSet(CommandOptions convertFolderIdFlag, PropertyDefinition propertyBagKey, ToServiceObjectFolderIdConvertSource sourceObject)
		{
			if (sourceObject != ToServiceObjectFolderIdConvertSource.StoreObject && sourceObject != ToServiceObjectFolderIdConvertSource.PropertyBag)
			{
				return false;
			}
			ToServiceObjectCommandSettingsBase commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettingsBase>();
			if (commandSettings == null || !commandSettings.CommandOptions.HasFlag(convertFolderIdFlag))
			{
				return false;
			}
			StoreId storeId = null;
			if (ToServiceObjectFolderIdConvertSource.PropertyBag == sourceObject)
			{
				ToServiceObjectForPropertyBagCommandSettings commandSettings2 = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
				if (commandSettings2 == null)
				{
					return false;
				}
				if (propertyBagKey == null)
				{
					return false;
				}
				IDictionary<PropertyDefinition, object> propertyBag = commandSettings2.PropertyBag;
				if (!PropertyCommand.TryGetValueFromPropertyBag<StoreId>(propertyBag, propertyBagKey, out storeId))
				{
					return false;
				}
			}
			else
			{
				ToServiceObjectCommandSettings commandSettings3 = base.GetCommandSettings<ToServiceObjectCommandSettings>();
				if (commandSettings3 == null)
				{
					return false;
				}
				storeId = this.GetIdFromObject(commandSettings3.StoreObject);
				if (storeId == null)
				{
					return false;
				}
			}
			ConcatenatedIdAndChangeKey concatenatedPublicFolderIdFromStoreId = FolderIdPropertyBase.GetConcatenatedPublicFolderIdFromStoreId(storeId);
			commandSettings.ServiceObject[this.commandContext.PropertyInformation] = this.CreateServiceObjectId(concatenatedPublicFolderIdFromStoreId.Id, concatenatedPublicFolderIdFromStoreId.ChangeKey);
			return true;
		}
	}
}
