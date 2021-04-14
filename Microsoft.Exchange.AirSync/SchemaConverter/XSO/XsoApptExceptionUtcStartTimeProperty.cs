using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoApptExceptionUtcStartTimeProperty : XsoProperty, IDateTimeProperty, IProperty
	{
		public XsoApptExceptionUtcStartTimeProperty(PropertyType t) : base(null, t)
		{
		}

		public ExDateTime DateTime
		{
			get
			{
				CalendarItemOccurrence calendarItemOccurrence = (CalendarItemOccurrence)base.XsoItem;
				return ExTimeZone.UtcTimeZone.ConvertDateTime(calendarItemOccurrence.OriginalStartTime);
			}
		}
	}
}
