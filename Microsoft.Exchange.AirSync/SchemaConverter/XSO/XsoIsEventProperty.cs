using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoIsEventProperty : XsoBooleanProperty
	{
		public XsoIsEventProperty() : base(CalendarItemBaseSchema.IsEvent, new PropertyDefinition[]
		{
			CalendarItemBaseSchema.MapiIsAllDayEvent
		})
		{
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			base.XsoItem[CalendarItemBaseSchema.MapiIsAllDayEvent] = ((IBooleanProperty)srcProperty).BooleanData;
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			if (!(base.XsoItem.TryGetProperty(CalendarItemBaseSchema.MapiIsAllDayEvent) is PropertyError))
			{
				base.XsoItem.DeleteProperties(new PropertyDefinition[]
				{
					CalendarItemBaseSchema.MapiIsAllDayEvent
				});
			}
		}
	}
}
