using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class BirthdayContactIdProperty : SmartPropertyDefinition
	{
		public BirthdayContactIdProperty() : base("BirthdayContactId", typeof(StoreObjectId), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(BirthdayContactIdProperty.EnclosedPropertyDefinition, PropertyDependencyType.AllRead)
		})
		{
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return IdProperty.NativeIdFilterToSmartIdFilter(filter, this, BirthdayContactIdProperty.EnclosedPropertyDefinition);
		}

		internal override void RegisterFilterTranslation()
		{
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ComparisonFilter));
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return IdProperty.SmartIdFilterToNativeIdFilter(filter, this, BirthdayContactIdProperty.EnclosedPropertyDefinition);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			StoreObjectId storeObjectId = value as StoreObjectId;
			if (storeObjectId == null)
			{
				throw new ArgumentException("value");
			}
			propertyBag.SetValue(BirthdayContactIdProperty.EnclosedPropertyDefinition, storeObjectId.ProviderLevelItemId);
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(BirthdayContactIdProperty.EnclosedPropertyDefinition);
			if (value is byte[])
			{
				return StoreObjectId.FromProviderSpecificId(value as byte[], StoreObjectType.Contact);
			}
			return new PropertyError(this, PropertyErrorCode.NotFound);
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return BirthdayContactIdProperty.EnclosedPropertyDefinition;
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.All;
			}
		}

		private static readonly NativeStorePropertyDefinition EnclosedPropertyDefinition = InternalSchema.BirthdayContactEntryId;
	}
}
