using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ItemBuilder
	{
		internal static T CreateNewItem<T>(StoreSession session, StoreId parentFolderId, ItemCreateInfo itemCreateInfo) where T : Item
		{
			return ItemBuilder.CreateNewItem<T>(session, parentFolderId, itemCreateInfo, CreateMessageType.Normal);
		}

		internal static T CreateNewItem<T>(StoreSession session, StoreId parentFolderId, ItemCreateInfo itemCreateInfo, CreateMessageType createMessageType) where T : Item
		{
			return ItemBuilder.CreateNewItem<T>(session, itemCreateInfo, () => Folder.InternalCreateMapiMessage(session, parentFolderId, createMessageType));
		}

		internal static T CreateNewItem<T>(StoreSession session, ItemCreateInfo itemCreateInfo, ItemBuilder.MapiMessageCreator mapiMessageCreator) where T : Item
		{
			T t = default(T);
			CoreItem coreItem = null;
			bool flag = false;
			T result;
			try
			{
				coreItem = ItemBuilder.CreateNewCoreItem(session, itemCreateInfo, true, mapiMessageCreator);
				t = (T)((object)itemCreateInfo.Creator(coreItem));
				flag = true;
				result = t;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(t);
					Util.DisposeIfPresent(coreItem);
				}
			}
			return result;
		}

		internal static CoreItem CreateNewCoreItem(StoreSession session, ItemCreateInfo itemCreateInfo, bool useAcr, ItemBuilder.MapiMessageCreator mapiMessageCreator)
		{
			return ItemBuilder.CreateNewCoreItem(session, itemCreateInfo, null, useAcr, mapiMessageCreator);
		}

		internal static CoreItem CreateNewCoreItem(StoreSession session, ItemCreateInfo itemCreateInfo, VersionedId itemId, bool useAcr, ItemBuilder.MapiMessageCreator mapiMessageCreator)
		{
			PersistablePropertyBag persistablePropertyBag = null;
			CoreItem coreItem = null;
			bool flag = false;
			StoreObjectId storeObjectId = null;
			byte[] changeKey = null;
			Origin origin = Origin.New;
			CoreItem result;
			try
			{
				persistablePropertyBag = ItemBuilder.ConstructItemPersistablePropertyBag(session, itemCreateInfo.Schema.AutoloadProperties, useAcr, itemCreateInfo.AcrProfile, mapiMessageCreator);
				if (itemId != null)
				{
					object obj = persistablePropertyBag.TryGetProperty(CoreItemSchema.ReadCnNew);
					if (obj is byte[] && ((byte[])obj).Length > 0)
					{
						changeKey = itemId.ChangeKeyAsByteArray();
						storeObjectId = itemId.ObjectId;
						origin = Origin.Existing;
					}
				}
				coreItem = new CoreItem(session, persistablePropertyBag, storeObjectId, changeKey, origin, ItemLevel.TopLevel, itemCreateInfo.Schema.AutoloadProperties, ItemBindOption.None);
				flag = true;
				result = coreItem;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(coreItem);
					Util.DisposeIfPresent(persistablePropertyBag);
				}
			}
			return result;
		}

		internal static T ConstructItem<T>(StoreSession session, StoreObjectId id, byte[] changeKey, ICollection<PropertyDefinition> propertiesToLoad, ItemBuilder.PropertyBagCreator propertyBagCreator, ItemCreateInfo.ItemCreator creator, Origin origin, ItemLevel itemLevel) where T : Item
		{
			PersistablePropertyBag persistablePropertyBag = null;
			CoreItem coreItem = null;
			T t = default(T);
			bool flag = false;
			T result;
			try
			{
				persistablePropertyBag = propertyBagCreator();
				coreItem = new CoreItem(session, persistablePropertyBag, id, changeKey, origin, itemLevel, propertiesToLoad, ItemBindOption.None);
				t = (T)((object)creator(coreItem));
				flag = true;
				result = t;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(t);
					Util.DisposeIfPresent(coreItem);
					Util.DisposeIfPresent(persistablePropertyBag);
				}
			}
			return result;
		}

		internal static T ItemBind<T>(StoreSession session, StoreId id, Schema expectedSchema, ICollection<PropertyDefinition> propertiesToLoad) where T : Item
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(id, "id");
			Util.ThrowOnNullArgument(expectedSchema, "expectedSchema");
			return ItemBuilder.ItemBind<T>(session, id, expectedSchema, null, ItemBindOption.None, propertiesToLoad);
		}

		internal static T ItemBind<T>(StoreSession session, StoreId storeId, Schema expectedSchema, ItemBuilder.MapiMessageCreator mapiMessageCreator, ItemBindOption itemBindOption, ICollection<PropertyDefinition> propertiesToLoad) where T : Item
		{
			propertiesToLoad = ItemBuilder.GetPropertiesToLoad(itemBindOption, expectedSchema, propertiesToLoad);
			bool flag = false;
			CoreItem coreItem = null;
			Item item = null;
			T t = default(T);
			T result;
			try
			{
				StoreObjectType storeObjectType = StoreObjectType.Unknown;
				coreItem = ItemBuilder.CoreItemBind(session, storeId, mapiMessageCreator, itemBindOption, propertiesToLoad, ref storeObjectType);
				ItemCreateInfo itemCreateInfo = ItemCreateInfo.GetItemCreateInfo(storeObjectType);
				item = itemCreateInfo.Creator(coreItem);
				t = item.DownCastStoreObject<T>();
				flag = true;
				result = t;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(t);
					Util.DisposeIfPresent(item);
					Util.DisposeIfPresent(coreItem);
				}
			}
			return result;
		}

		internal static MessageItem ItemBindAsMessage(StoreSession session, StoreId storeId, ItemBuilder.MapiMessageCreator mapiMessageCreator, ItemBindOption itemBindOption, ICollection<PropertyDefinition> propertiesToLoad)
		{
			ItemCreateInfo messageItemInfo = ItemCreateInfo.MessageItemInfo;
			propertiesToLoad = ItemBuilder.GetPropertiesToLoad(itemBindOption, messageItemInfo.Schema, propertiesToLoad);
			CoreItem coreItem = null;
			MessageItem messageItem = null;
			bool flag = false;
			MessageItem result;
			try
			{
				StoreObjectType storeObjectType = StoreObjectType.Message;
				coreItem = ItemBuilder.CoreItemBind(session, storeId, mapiMessageCreator, itemBindOption, propertiesToLoad, ref storeObjectType);
				messageItem = (MessageItem)messageItemInfo.Creator(coreItem);
				flag = true;
				result = messageItem;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(messageItem);
					Util.DisposeIfPresent(coreItem);
				}
			}
			return result;
		}

		internal static CoreItem CoreItemBind(StoreSession session, StoreId storeId, ItemBuilder.MapiMessageCreator mapiMessageCreator, ItemBindOption itemBindOption, ICollection<PropertyDefinition> propertiesToLoad, ref StoreObjectType storeObjectType)
		{
			Util.ThrowOnNullArgument(session, "session");
			EnumValidator.ThrowIfInvalid<ItemBindOption>(itemBindOption);
			Util.ThrowOnNullArgument(propertiesToLoad, "propertiesToLoad");
			bool flag = false;
			MapiProp mapiProp = null;
			PersistablePropertyBag persistablePropertyBag = null;
			AcrPropertyBag acrPropertyBag = null;
			CoreItem coreItem = null;
			CoreItem result2;
			using (CallbackContext callbackContext = new CallbackContext(session))
			{
				try
				{
					session.OnBeforeItemChange(ItemChangeOperation.ItemBind, session, storeId, coreItem, callbackContext);
					StoreObjectId storeObjectId;
					byte[] array;
					StoreId.SplitStoreObjectIdAndChangeKey(storeId, out storeObjectId, out array);
					session.CheckSystemFolderAccess(storeObjectId);
					if (storeObjectId != null && !IdConverter.IsMessageId(storeObjectId))
					{
						throw new ArgumentException(ServerStrings.ExInvalidItemId);
					}
					bool flag2 = false;
					OccurrenceStoreObjectId occurrenceStoreObjectId = storeObjectId as OccurrenceStoreObjectId;
					IPropertyBagFactory propertyBagFactory;
					if (occurrenceStoreObjectId != null)
					{
						persistablePropertyBag = Item.CreateOccurrencePropertyBag(session, occurrenceStoreObjectId, propertiesToLoad);
						storeObjectType = StoreObjectType.CalendarItemOccurrence;
						flag2 = true;
						propertyBagFactory = new OccurrenceBagFactory(session, occurrenceStoreObjectId);
					}
					else
					{
						if (mapiMessageCreator != null)
						{
							mapiProp = mapiMessageCreator();
						}
						else if ((itemBindOption & ItemBindOption.SoftDeletedItem) == ItemBindOption.SoftDeletedItem)
						{
							mapiProp = session.GetMapiProp(storeObjectId, OpenEntryFlags.BestAccess | OpenEntryFlags.DeferredErrors | OpenEntryFlags.ShowSoftDeletes);
						}
						else
						{
							mapiProp = session.GetMapiProp(storeObjectId);
						}
						persistablePropertyBag = new StoreObjectPropertyBag(session, mapiProp, propertiesToLoad);
						StoreObjectType storeObjectType2 = ItemBuilder.ReadStoreObjectTypeFromPropertyBag(persistablePropertyBag);
						if (storeObjectType2 == storeObjectType)
						{
							flag2 = true;
						}
						else
						{
							storeObjectType = storeObjectType2;
						}
						propertyBagFactory = new RetryBagFactory(session);
						if (storeObjectId != null && storeObjectId.ObjectType != storeObjectType)
						{
							storeObjectId = StoreObjectId.FromProviderSpecificId(storeObjectId.ProviderLevelItemId, storeObjectType);
						}
					}
					ItemBuilder.CheckPrivateItem(session, persistablePropertyBag);
					ItemCreateInfo itemCreateInfo = ItemCreateInfo.GetItemCreateInfo(storeObjectType);
					if (flag2)
					{
						propertiesToLoad = null;
					}
					else
					{
						propertiesToLoad = ItemBuilder.GetPropertiesToLoad(itemBindOption, itemCreateInfo.Schema, propertiesToLoad);
					}
					acrPropertyBag = new AcrPropertyBag(persistablePropertyBag, itemCreateInfo.AcrProfile, storeObjectId, propertyBagFactory, array);
					coreItem = new CoreItem(session, acrPropertyBag, storeObjectId, array, Origin.Existing, ItemLevel.TopLevel, propertiesToLoad, itemBindOption);
					flag = true;
					ConflictResolutionResult result = flag ? ConflictResolutionResult.Success : ConflictResolutionResult.Failure;
					session.OnAfterItemChange(ItemChangeOperation.ItemBind, session, storeId, coreItem, result, callbackContext);
					result2 = coreItem;
				}
				finally
				{
					if (!flag)
					{
						Util.DisposeIfPresent(coreItem);
						Util.DisposeIfPresent(acrPropertyBag);
						Util.DisposeIfPresent(persistablePropertyBag);
						Util.DisposeIfPresent(mapiProp);
					}
				}
			}
			return result2;
		}

		internal static StoreObjectType ReadStoreObjectTypeFromPropertyBag(ICorePropertyBag propertyBag)
		{
			object propertyValue = propertyBag.TryGetProperty(CoreItemSchema.ItemClass);
			string text;
			if (PropertyError.IsPropertyValueTooBig(propertyValue) || PropertyError.IsPropertyNotFound(propertyValue))
			{
				text = string.Empty;
			}
			else
			{
				text = PropertyBag.CheckPropertyValue<string>(CoreItemSchema.ItemClass, propertyValue);
			}
			StoreObjectType objectType = ObjectClass.GetObjectType(text);
			for (int i = 0; i < ItemBuilder.storeObjectTypeDetectionChain.Length; i++)
			{
				StoreObjectType? storeObjectType = ItemBuilder.storeObjectTypeDetectionChain[i](propertyBag, text, objectType);
				if (storeObjectType != null)
				{
					return storeObjectType.Value;
				}
			}
			return objectType;
		}

		private static StoreObjectType? TryDetectFolderTreeDataStoreObjectType(ICorePropertyBag propertyBag, string itemClass, StoreObjectType detectedType)
		{
			StoreObjectType? result = null;
			if (ObjectClass.IsFolderTreeData(itemClass))
			{
				propertyBag.Load(new StorePropertyDefinition[]
				{
					FolderTreeDataSchema.GroupSection,
					FolderTreeDataSchema.Type
				});
				FolderTreeDataSection? valueAsNullable = propertyBag.GetValueAsNullable<FolderTreeDataSection>(FolderTreeDataSchema.GroupSection);
				FolderTreeDataType? valueAsNullable2 = propertyBag.GetValueAsNullable<FolderTreeDataType>(FolderTreeDataSchema.Type);
				if (valueAsNullable == FolderTreeDataSection.Calendar)
				{
					if (valueAsNullable2 == FolderTreeDataType.Header)
					{
						result = new StoreObjectType?(StoreObjectType.CalendarGroup);
					}
					else if (valueAsNullable2 == FolderTreeDataType.NormalFolder || valueAsNullable2 == FolderTreeDataType.SharedFolder)
					{
						result = new StoreObjectType?(StoreObjectType.CalendarGroupEntry);
					}
				}
				else if (valueAsNullable == FolderTreeDataSection.First)
				{
					result = new StoreObjectType?(StoreObjectType.FavoriteFolderEntry);
				}
				else if (valueAsNullable == FolderTreeDataSection.Tasks)
				{
					if (valueAsNullable2 == FolderTreeDataType.Header)
					{
						result = new StoreObjectType?(StoreObjectType.TaskGroup);
					}
					else if (valueAsNullable2 == FolderTreeDataType.NormalFolder)
					{
						result = new StoreObjectType?(StoreObjectType.TaskGroupEntry);
					}
				}
			}
			return result;
		}

		private static StoreObjectType? TryDetectShortcutMessageEntryStoreObjectType(ICorePropertyBag propertyBag, string itemClass, StoreObjectType detectedType)
		{
			if (detectedType != StoreObjectType.Message || !ObjectClass.IsShortcutMessageEntry(propertyBag.GetValueOrDefault<int>(CoreItemSchema.FavLevelMask, -1)))
			{
				return null;
			}
			return new StoreObjectType?(StoreObjectType.ShortcutMessage);
		}

		private static StoreObjectType? TryDetectRightsManagementStoreObjectType(ICorePropertyBag propertyBag, string itemClass, StoreObjectType detectedType)
		{
			if (detectedType != StoreObjectType.Message || !ObjectClass.IsRightsManagedContentClass(propertyBag.GetValueOrDefault<string>(CoreObjectSchema.ContentClass, null)))
			{
				return null;
			}
			return new StoreObjectType?(StoreObjectType.RightsManagedMessage);
		}

		internal static ICollection<PropertyDefinition> GetPropertiesToLoad(ItemBindOption itemBindOption, Schema schema, ICollection<PropertyDefinition> requestedProperties)
		{
			ICollection<PropertyDefinition> first = ((itemBindOption & ItemBindOption.LoadRequiredPropertiesOnly) == ItemBindOption.LoadRequiredPropertiesOnly) ? schema.RequiredAutoloadProperties : schema.AutoloadProperties;
			return InternalSchema.Combine<PropertyDefinition>(first, requestedProperties);
		}

		internal static PersistablePropertyBag ConstructItemPersistablePropertyBag(StoreSession session, ICollection<PropertyDefinition> propertiesToLoad, bool createAcrPropertyBag, AcrProfile acrProfile, ItemBuilder.MapiMessageCreator mapiMessageCreator)
		{
			MapiMessage mapiMessage = null;
			PersistablePropertyBag persistablePropertyBag = null;
			PersistablePropertyBag persistablePropertyBag2 = null;
			bool flag = false;
			PersistablePropertyBag result;
			try
			{
				mapiMessage = mapiMessageCreator();
				persistablePropertyBag = new StoreObjectPropertyBag(session, mapiMessage, propertiesToLoad);
				PersistablePropertyBag persistablePropertyBag3;
				if (createAcrPropertyBag)
				{
					persistablePropertyBag2 = new AcrPropertyBag(persistablePropertyBag, acrProfile, null, new RetryBagFactory(session), null);
					persistablePropertyBag3 = persistablePropertyBag2;
				}
				else
				{
					persistablePropertyBag3 = persistablePropertyBag;
				}
				flag = true;
				result = persistablePropertyBag3;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(persistablePropertyBag2);
					Util.DisposeIfPresent(persistablePropertyBag);
					Util.DisposeIfPresent(mapiMessage);
				}
			}
			return result;
		}

		private static void CheckPrivateItem(StoreSession session, PropertyBag propertyBag)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession != null && mailboxSession.FilterPrivateItems)
			{
				Sensitivity? valueAsNullable = propertyBag.GetValueAsNullable<Sensitivity>(CoreItemSchema.MapiSensitivity);
				if (valueAsNullable != null && valueAsNullable.Value == Sensitivity.Private)
				{
					throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
				}
			}
		}

		private static readonly Func<ICorePropertyBag, string, StoreObjectType, StoreObjectType?>[] storeObjectTypeDetectionChain = new Func<ICorePropertyBag, string, StoreObjectType, StoreObjectType?>[]
		{
			new Func<ICorePropertyBag, string, StoreObjectType, StoreObjectType?>(ItemBuilder.TryDetectFolderTreeDataStoreObjectType),
			new Func<ICorePropertyBag, string, StoreObjectType, StoreObjectType?>(ItemBuilder.TryDetectShortcutMessageEntryStoreObjectType),
			new Func<ICorePropertyBag, string, StoreObjectType, StoreObjectType?>(ItemBuilder.TryDetectRightsManagementStoreObjectType)
		};

		internal delegate MapiMessage MapiMessageCreator();

		internal delegate PersistablePropertyBag PropertyBagCreator();
	}
}
