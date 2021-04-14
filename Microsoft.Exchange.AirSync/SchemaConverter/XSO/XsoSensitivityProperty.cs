using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoSensitivityProperty : XsoIntegerProperty
	{
		public XsoSensitivityProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
		}

		public XsoSensitivityProperty(StorePropertyDefinition propertyDef, PropertyType type) : base(propertyDef, type)
		{
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			if (!(base.XsoItem is CalendarItemBase) && !(base.XsoItem is Task))
			{
				throw new UnexpectedTypeException("CalendarItemBase or Task", base.XsoItem);
			}
			Item item = (Item)base.XsoItem;
			if (!(item.TryGetProperty(base.PropertyDef) is PropertyError))
			{
				item.Sensitivity = Sensitivity.Normal;
				return;
			}
		}
	}
}
