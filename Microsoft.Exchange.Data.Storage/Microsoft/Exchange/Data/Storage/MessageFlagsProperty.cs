using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class MessageFlagsProperty : FlagsProperty
	{
		internal MessageFlagsProperty(string displayName, NativeStorePropertyDefinition nativeProperty, int flag) : base(displayName, nativeProperty, flag, PropertyDefinitionConstraint.None)
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			Item item = propertyBag.Context.StoreObject as Item;
			if (item == null)
			{
				return base.InternalTryGetValue(propertyBag);
			}
			bool? flagsApiProperties = item.GetFlagsApiProperties(base.NativeProperty, base.Flag);
			if (flagsApiProperties == null)
			{
				return base.InternalTryGetValue(propertyBag);
			}
			return flagsApiProperties.GetValueOrDefault();
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (!(value is bool))
			{
				string message = ServerStrings.ExInvalidValueForFlagsCalculatedProperty(base.Flag);
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), message);
				throw new ArgumentException(message);
			}
			Item item = propertyBag.Context.StoreObject as Item;
			if (item != null)
			{
				item.SetFlagsApiProperties(base.NativeProperty, base.Flag, (bool)value);
				return;
			}
			base.InternalSetValue(propertyBag, value);
		}
	}
}
