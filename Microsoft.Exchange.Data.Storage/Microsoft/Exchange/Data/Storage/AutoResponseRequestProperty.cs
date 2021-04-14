using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class AutoResponseRequestProperty : SmartPropertyDefinition
	{
		internal AutoResponseRequestProperty(string displayName, NativeStorePropertyDefinition nativeProp) : base(displayName, nativeProp.Type, PropertyFlags.Sortable, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(nativeProp, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.AutoResponseSuppressInternal, PropertyDependencyType.NeedToReadForWrite)
		})
		{
			this.nativeProp = nativeProp;
			this.suppressMask = AutoResponseSuppressProperty.SuppressionMapping[nativeProp];
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return propertyBag.GetValue(this.nativeProp);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			propertyBag.SetValueWithFixup(this.nativeProp, (bool)value && !this.IsSuppressed(propertyBag));
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return this.nativeProp;
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.All;
			}
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, this.nativeProp);
		}

		private bool IsSuppressed(PropertyBag.BasicPropertyStore propertyBag)
		{
			int num;
			return Util.TryConvertTo<int>(propertyBag.GetValue(InternalSchema.AutoResponseSuppressInternal), out num) && (num & (int)this.suppressMask) == (int)this.suppressMask;
		}

		private readonly NativeStorePropertyDefinition nativeProp;

		private readonly AutoResponseSuppress suppressMask;
	}
}
