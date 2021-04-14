using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoGuidProperty : XsoProperty, IStringProperty, IProperty
	{
		public XsoGuidProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
		}

		public XsoGuidProperty(StorePropertyDefinition propertyDef, PropertyType type) : base(propertyDef, type)
		{
		}

		public XsoGuidProperty(StorePropertyDefinition propertyDef, PropertyType type, PropertyDefinition[] prefetchPropDef) : base(propertyDef, type, prefetchPropDef)
		{
		}

		public virtual string StringData
		{
			get
			{
				return ((Guid)base.XsoItem.TryGetProperty(base.PropertyDef)).ToString();
			}
		}
	}
}
