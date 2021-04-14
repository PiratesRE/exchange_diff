using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoUtcDateTimeProperty : XsoProperty, IDateTimeProperty, IProperty
	{
		public XsoUtcDateTimeProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
		}

		public XsoUtcDateTimeProperty(StorePropertyDefinition propertyDef, PropertyType type) : base(propertyDef, type)
		{
		}

		public XsoUtcDateTimeProperty(StorePropertyDefinition propertyDef, PropertyDefinition[] prefetchPropDef) : base(propertyDef, prefetchPropDef)
		{
		}

		public virtual ExDateTime DateTime
		{
			get
			{
				return ExTimeZone.UtcTimeZone.ConvertDateTime((ExDateTime)base.XsoItem.TryGetProperty(base.PropertyDef));
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			if (PropertyState.SetToDefault == srcProperty.State)
			{
				throw new ConversionException("Object type does not support setting to default");
			}
			ExDateTime exDateTime = ((IDateTimeProperty)srcProperty).DateTime;
			exDateTime = ExTimeZone.UtcTimeZone.ConvertDateTime(exDateTime);
			base.XsoItem[base.PropertyDef] = exDateTime;
		}
	}
}
