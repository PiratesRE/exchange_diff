using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class PersonIdProperty : SmartPropertyDefinition
	{
		internal PersonIdProperty() : base("PersonId", typeof(PersonId), PropertyFlags.None, PropertyDefinitionConstraint.None, PersonIdProperty.PropertyDependencies)
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(InternalSchema.ConversationIndex);
			byte[] array = value as byte[];
			if (array == null)
			{
				PropertyError propertyError = (PropertyError)value;
				if (propertyError.PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
				{
					return new PropertyError(this, PropertyErrorCode.CorruptedData);
				}
				return new PropertyError(this, propertyError.PropertyErrorCode);
			}
			else
			{
				if (array.Length < 22)
				{
					return new PropertyError(this, PropertyErrorCode.CorruptedData);
				}
				byte[] array2 = new byte[16];
				Array.Copy(array, 6, array2, 0, 16);
				object result;
				try
				{
					result = PersonId.Create(array2);
				}
				catch (CorruptDataException)
				{
					result = new PropertyError(this, PropertyErrorCode.CorruptedData);
				}
				return result;
			}
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			PersonId personId = value as PersonId;
			if (personId == null)
			{
				throw new ArgumentException("value");
			}
			byte[] array = new byte[22];
			array[0] = 1;
			Array.Copy(personId.GetBytes(), 0, array, 6, 16);
			propertyBag.SetValue(InternalSchema.ConversationIndex, array);
			propertyBag.SetValue(InternalSchema.ConversationIndexTracking, true);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null || !comparisonFilter.Property.Equals(InternalSchema.MapiConversationId))
			{
				throw base.CreateInvalidFilterConversionException(filter);
			}
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, this, PersonId.Create((byte[])comparisonFilter.PropertyValue));
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null || !comparisonFilter.Property.Equals(this))
			{
				throw base.CreateInvalidFilterConversionException(filter);
			}
			PersonId personId = (PersonId)comparisonFilter.PropertyValue;
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, InternalSchema.MapiConversationId, personId.GetBytes());
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return InternalSchema.MapiConversationId;
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.All;
			}
		}

		private const int MinimumConversationIndexLength = 22;

		private const int PersonIdIndexIntoConversationIndexBlob = 6;

		private const int PersonIdLengthInBytes = 16;

		private static readonly PropertyDependency[] PropertyDependencies = new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.ConversationIndex, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.ConversationIndexTracking, PropertyDependencyType.AllRead)
		};
	}
}
