using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ItemIdProperty : IdProperty
	{
		public ItemIdProperty() : base("ItemId", typeof(VersionedId), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.EntryId, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ChangeKey, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override StoreObjectType GetStoreObjectType(PropertyBag.BasicPropertyStore propertyBag)
		{
			return ObjectClass.GetObjectType(propertyBag.GetValue(InternalSchema.ItemClass) as string);
		}

		protected override bool IsCompatibleId(StoreId id, ICoreObject coreObject)
		{
			return (coreObject == null || coreObject is ICoreItem) && IdConverter.IsMessageId(StoreId.GetStoreObjectId(id));
		}
	}
}
