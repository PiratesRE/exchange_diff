using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoPersistentStringProperty : XsoStringProperty
	{
		public XsoPersistentStringProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
		}

		public XsoPersistentStringProperty(StorePropertyDefinition propertyDef, PropertyType type) : base(propertyDef, type)
		{
		}

		public XsoPersistentStringProperty(StorePropertyDefinition propertyDef, PropertyType type, params PropertyDefinition[] prefechProperties) : base(propertyDef, type, prefechProperties)
		{
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			base.XsoItem[base.PropertyDef] = string.Empty;
		}
	}
}
