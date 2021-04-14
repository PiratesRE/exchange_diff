using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class StoreObjectIdCollectionProperty : SmartPropertyDefinition
	{
		internal StoreObjectIdCollectionProperty(NativeStorePropertyDefinition propertyDefinition, PropertyFlags propertyFlags, string displayName) : base(displayName, typeof(StoreObjectId[]), propertyFlags, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(propertyDefinition, PropertyDependencyType.NeedForRead)
		})
		{
			this.enclosedPropertyDefinition = propertyDefinition;
			if (propertyDefinition.Type != typeof(byte[][]))
			{
				ExDiagnostics.FailFast("Can't create StoreObjectIdCollectionProperty on non byte[][] typed properties", false);
			}
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			byte[][] array = propertyBag.GetValue(this.enclosedPropertyDefinition) as byte[][];
			if (array != null)
			{
				StoreObjectId[] array2 = new StoreObjectId[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = StoreObjectId.FromProviderSpecificId(array[i], StoreObjectType.Unknown);
				}
				return array2;
			}
			return new PropertyError(this, PropertyErrorCode.NotFound);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (value != null)
			{
				StoreObjectId[] array = value as StoreObjectId[];
				ArgumentValidator.ThrowIfNull("value", array);
				byte[][] array2 = new byte[array.Length][];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = array[i].ProviderLevelItemId;
				}
				propertyBag.SetValue(this.enclosedPropertyDefinition, array2);
				return;
			}
			propertyBag.Delete(this.enclosedPropertyDefinition);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null)
			{
				return base.SinglePropertySmartFilterToNativeFilter(filter, this.enclosedPropertyDefinition);
			}
			StoreObjectId storeObjectId = comparisonFilter.PropertyValue as StoreObjectId;
			if (storeObjectId == null)
			{
				throw new NotSupportedException("FolderItemIdCollectionProperty only supports StoreObjectId in filters");
			}
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, this.enclosedPropertyDefinition, storeObjectId.ProviderLevelItemId);
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return base.Capabilities | StorePropertyCapabilities.CanQuery;
			}
		}

		private readonly NativeStorePropertyDefinition enclosedPropertyDefinition;
	}
}
