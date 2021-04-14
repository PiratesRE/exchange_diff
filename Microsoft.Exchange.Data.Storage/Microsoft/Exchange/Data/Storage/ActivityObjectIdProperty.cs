using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ActivityObjectIdProperty : SmartPropertyDefinition
	{
		internal ActivityObjectIdProperty(string propertyName, NativeStorePropertyDefinition backingPropertyDefinition) : base(propertyName, typeof(StoreObjectId), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(backingPropertyDefinition, PropertyDependencyType.NeedForRead)
		})
		{
			this.backingPropertyDefinition = backingPropertyDefinition;
		}

		protected sealed override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(this.backingPropertyDefinition);
		}

		protected sealed override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			StoreObjectId storeObjectId = value as StoreObjectId;
			if (storeObjectId == null)
			{
				throw new ArgumentException("value", "Must be a non-null StoreObjectId");
			}
			byte[] bytes = storeObjectId.GetBytes();
			propertyBag.SetValue(this.backingPropertyDefinition, bytes);
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(this.backingPropertyDefinition);
			byte[] array = value as byte[];
			if (array != null)
			{
				object result;
				try
				{
					result = StoreObjectId.Parse(array, 0);
				}
				catch (CorruptDataException)
				{
					result = new PropertyError(this, PropertyErrorCode.CorruptedData);
				}
				return result;
			}
			PropertyError propertyError = (PropertyError)value;
			if (propertyError.PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
			{
				return new PropertyError(this, PropertyErrorCode.CorruptedData);
			}
			return new PropertyError(this, propertyError.PropertyErrorCode);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			SinglePropertyFilter singlePropertyFilter = filter as SinglePropertyFilter;
			if (singlePropertyFilter != null && singlePropertyFilter.Property.Equals(this.backingPropertyDefinition))
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				if (comparisonFilter != null)
				{
					return new ComparisonFilter(comparisonFilter.ComparisonOperator, this, StoreObjectId.Parse((byte[])comparisonFilter.PropertyValue, 0));
				}
				ExistsFilter existsFilter = filter as ExistsFilter;
				if (existsFilter != null)
				{
					return new ExistsFilter(this);
				}
			}
			return null;
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter != null)
			{
				return new ComparisonFilter(comparisonFilter.ComparisonOperator, this.backingPropertyDefinition, ((StoreObjectId)comparisonFilter.PropertyValue).GetBytes());
			}
			ExistsFilter existsFilter = filter as ExistsFilter;
			if (existsFilter != null)
			{
				return new ExistsFilter(this.backingPropertyDefinition);
			}
			throw base.CreateInvalidFilterConversionException(filter);
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return this.backingPropertyDefinition;
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.All;
			}
		}

		private NativeStorePropertyDefinition backingPropertyDefinition;
	}
}
