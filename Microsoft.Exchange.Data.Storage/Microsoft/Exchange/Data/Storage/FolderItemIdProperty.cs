using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class FolderItemIdProperty : SmartPropertyDefinition
	{
		internal FolderItemIdProperty(NativeStorePropertyDefinition propertyDefinition, string displayName) : base(displayName, typeof(StoreObjectId), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(propertyDefinition, PropertyDependencyType.NeedForRead)
		})
		{
			this.enclosedPropertyDefinition = propertyDefinition;
			if (propertyDefinition.Type != typeof(byte[]))
			{
				ExDiagnostics.FailFast("Can't create FolderItemIdProperty on non byte[] typed properties", false);
			}
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(this.enclosedPropertyDefinition);
			if (value is byte[])
			{
				return StoreObjectId.FromProviderSpecificId((byte[])value, StoreObjectType.Unknown);
			}
			return new PropertyError(this, PropertyErrorCode.NotFound);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return IdProperty.SmartIdFilterToNativeIdFilter(filter, this, this.enclosedPropertyDefinition);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return IdProperty.NativeIdFilterToSmartIdFilter(filter, this, this.enclosedPropertyDefinition);
		}

		internal override void RegisterFilterTranslation()
		{
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ComparisonFilter));
		}

		private NativeStorePropertyDefinition enclosedPropertyDefinition;
	}
}
