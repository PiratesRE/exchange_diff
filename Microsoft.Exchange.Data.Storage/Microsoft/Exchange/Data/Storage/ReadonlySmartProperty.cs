using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ReadonlySmartProperty : SmartPropertyDefinition
	{
		internal ReadonlySmartProperty(NativeStorePropertyDefinition propertyDefinition) : base(propertyDefinition.Name, propertyDefinition.Type, PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(propertyDefinition, PropertyDependencyType.NeedForRead)
		})
		{
			this.enclosedProperty = propertyDefinition;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return propertyBag.GetValue(this.enclosedProperty);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, this.enclosedProperty);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return base.SinglePropertyNativeFilterToSmartFilter(filter, this.enclosedProperty);
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return this.enclosedProperty;
		}

		internal override void RegisterFilterTranslation()
		{
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(SinglePropertyFilter));
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return this.enclosedProperty.Capabilities;
			}
		}

		private readonly NativeStorePropertyDefinition enclosedProperty;
	}
}
