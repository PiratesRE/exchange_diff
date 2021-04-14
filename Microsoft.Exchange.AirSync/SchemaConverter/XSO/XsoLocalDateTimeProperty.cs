using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoLocalDateTimeProperty : XsoProperty, IDateTimeProperty, IProperty
	{
		public XsoLocalDateTimeProperty(StorePropertyDefinition timestampDef, StorePropertyDefinition timeZoneDef) : base(timestampDef)
		{
			this.timeZoneDef = timeZoneDef;
		}

		public XsoLocalDateTimeProperty(StorePropertyDefinition timestampDef, StorePropertyDefinition timeZoneDef, PropertyType type) : base(timestampDef, type)
		{
			this.timeZoneDef = timeZoneDef;
		}

		public ExDateTime DateTime
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
			ExTimeZone exTimeZone = null;
			if (this.timeZoneDef != null)
			{
				exTimeZone = base.XsoItem.GetValueOrDefault<ExTimeZone>(this.timeZoneDef);
			}
			if (exTimeZone == null || exTimeZone.Equals(ExTimeZone.UtcTimeZone))
			{
				exTimeZone = TimeZoneHelper.GetPromotedTimeZoneFromItem(base.XsoItem as Item);
			}
			ExDateTime dateTime = ((IDateTimeProperty)srcProperty).DateTime;
			ExDateTime exDateTime = exTimeZone.ConvertDateTime(dateTime);
			base.XsoItem[base.PropertyDef] = exDateTime;
		}

		private StorePropertyDefinition timeZoneDef;
	}
}
