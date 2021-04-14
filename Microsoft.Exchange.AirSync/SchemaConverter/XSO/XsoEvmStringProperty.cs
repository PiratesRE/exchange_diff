using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoEvmStringProperty : XsoStringProperty
	{
		public XsoEvmStringProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
			base.SupportedItemClasses = Constants.EvmSupportedItemClassPrefixes;
		}

		public XsoEvmStringProperty(StorePropertyDefinition propertyDef, PropertyType type) : base(propertyDef, type)
		{
			base.SupportedItemClasses = Constants.EvmSupportedItemClassPrefixes;
		}

		public XsoEvmStringProperty(StorePropertyDefinition propertyDef, PropertyType type, params PropertyDefinition[] prefechProperties) : base(propertyDef, type, prefechProperties)
		{
			base.SupportedItemClasses = Constants.EvmSupportedItemClassPrefixes;
		}
	}
}
