using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class AutoResponseSuppressProperty : SmartPropertyDefinition
	{
		internal AutoResponseSuppressProperty() : base("AutoResponseSuppress", InternalSchema.AutoResponseSuppressInternal.Type, PropertyFlags.Sortable, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.AutoResponseSuppressInternal, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return propertyBag.GetValue(InternalSchema.AutoResponseSuppressInternal);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			AutoResponseSuppress autoResponseSuppress = (AutoResponseSuppress)value;
			foreach (KeyValuePair<NativeStorePropertyDefinition, AutoResponseSuppress> keyValuePair in AutoResponseSuppressProperty.SuppressionMapping)
			{
				if ((autoResponseSuppress & keyValuePair.Value) == keyValuePair.Value)
				{
					propertyBag.SetValueWithFixup(keyValuePair.Key, false);
				}
			}
			propertyBag.SetValueWithFixup(InternalSchema.AutoResponseSuppressInternal, (int)value);
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.CanSortBy | StorePropertyCapabilities.CanGroupBy;
			}
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return InternalSchema.AutoResponseSuppressInternal;
		}

		internal static readonly Dictionary<NativeStorePropertyDefinition, AutoResponseSuppress> SuppressionMapping = Util.AddElements<Dictionary<NativeStorePropertyDefinition, AutoResponseSuppress>, KeyValuePair<NativeStorePropertyDefinition, AutoResponseSuppress>>(new Dictionary<NativeStorePropertyDefinition, AutoResponseSuppress>(), new KeyValuePair<NativeStorePropertyDefinition, AutoResponseSuppress>[]
		{
			new KeyValuePair<NativeStorePropertyDefinition, AutoResponseSuppress>(InternalSchema.IsDeliveryReceiptRequestedInternal, AutoResponseSuppress.DR),
			new KeyValuePair<NativeStorePropertyDefinition, AutoResponseSuppress>(InternalSchema.IsNonDeliveryReceiptRequestedInternal, AutoResponseSuppress.NDR),
			new KeyValuePair<NativeStorePropertyDefinition, AutoResponseSuppress>(InternalSchema.IsReadReceiptRequestedInternal, AutoResponseSuppress.RN),
			new KeyValuePair<NativeStorePropertyDefinition, AutoResponseSuppress>(InternalSchema.IsNotReadReceiptRequestedInternal, AutoResponseSuppress.NRN)
		});
	}
}
