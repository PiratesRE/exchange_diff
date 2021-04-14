using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ReminderAdjustmentProperty : SmartPropertyDefinition
	{
		internal ReminderAdjustmentProperty(string displayName, NativeStorePropertyDefinition nativeProp) : base(displayName, nativeProp.Type, PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(nativeProp, PropertyDependencyType.NeedForRead)
		})
		{
			this.nativeProp = nativeProp;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return propertyBag.GetValue(this.nativeProp);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			propertyBag.SetValueWithFixup(this.nativeProp, value);
			Reminder.Adjust(propertyBag.Context.StoreObject);
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.All;
			}
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return this.nativeProp;
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, this.nativeProp);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return base.SinglePropertyNativeFilterToSmartFilter(filter, this.nativeProp);
		}

		private readonly NativeStorePropertyDefinition nativeProp;
	}
}
