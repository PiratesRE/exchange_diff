using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.TypeConversion.PropertyTranslationRules;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.TypeConversion.Translators
{
	internal class ItemTranslator<TStoreItem, TItemEntity, TItemSchema> : StorageEntityTranslator<TStoreItem, TItemEntity, TItemSchema>, IGenericItemTranslator where TStoreItem : IItem where TItemEntity : Item<TItemSchema>, new() where TItemSchema : ItemSchema, new()
	{
		protected ItemTranslator(IEnumerable<ITranslationRule<TStoreItem, TItemEntity>> additionalRules = null) : base(ItemTranslator<TStoreItem, TItemEntity, TItemSchema>.CreateTranslationRules().AddRules(additionalRules))
		{
		}

		public new static ItemTranslator<TStoreItem, TItemEntity, TItemSchema> Instance
		{
			get
			{
				return ItemTranslator<TStoreItem, TItemEntity, TItemSchema>.SingletonInstance;
			}
		}

		IItem IGenericItemTranslator.ConvertToEntity(IItem storageItem)
		{
			return base.ConvertToEntity((TStoreItem)((object)storageItem));
		}

		private static List<ITranslationRule<TStoreItem, TItemEntity>> CreateTranslationRules()
		{
			return new List<ITranslationRule<TStoreItem, TItemEntity>>
			{
				ItemAccessors<TStoreItem>.Body.MapTo(Item<TItemSchema>.Accessors.Body),
				ItemAccessors<TStoreItem>.Preview.MapTo(Item<TItemSchema>.Accessors.Preview),
				ItemAccessors<TStoreItem>.ReceivedTime.MapTo(Item<TItemSchema>.Accessors.ReceivedTime),
				ItemAccessors<TStoreItem>.Categories.MapTo(Item<TItemSchema>.Accessors.Categories),
				ItemAccessors<TStoreItem>.HasAttachment.MapTo(Item<TItemSchema>.Accessors.HasAttachments),
				ItemAccessors<TStoreItem>.Importance.MapTo(Item<TItemSchema>.Accessors.Importance, default(ImportanceConverter)),
				ItemAccessors<TStoreItem>.Sensitivity.MapTo(Item<TItemSchema>.Accessors.Sensitivity, default(SensitivityConverter)),
				ItemAccessors<TStoreItem>.Subject.MapTo(Item<TItemSchema>.Accessors.Subject),
				ItemAccessors<TStoreItem>.LastModifiedTime.MapTo(Item<TItemSchema>.Accessors.LastModifiedTime),
				ItemAccessors<TStoreItem>.DateTimeCreated.MapTo(Item<TItemSchema>.Accessors.DateTimeCreated),
				new AttachmentTranslationRule<TStoreItem, TItemEntity, TItemSchema>()
			};
		}

		private static readonly ItemTranslator<TStoreItem, TItemEntity, TItemSchema> SingletonInstance = new ItemTranslator<TStoreItem, TItemEntity, TItemSchema>(null);
	}
}
