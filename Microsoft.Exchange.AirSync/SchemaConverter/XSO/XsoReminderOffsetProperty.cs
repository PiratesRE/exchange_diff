using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoReminderOffsetProperty : XsoIntegerProperty
	{
		public XsoReminderOffsetProperty() : base(ItemSchema.ReminderMinutesBeforeStart, new PropertyDefinition[]
		{
			ItemSchema.ReminderIsSet,
			ItemSchema.ReminderMinutesBeforeStart
		})
		{
		}

		public override int IntegerData
		{
			get
			{
				CalendarItemBase calendarItemBase = base.XsoItem as CalendarItemBase;
				if (!calendarItemBase.Reminder.IsSet)
				{
					return -1;
				}
				int minutesBeforeStart = calendarItemBase.Reminder.MinutesBeforeStart;
				if (minutesBeforeStart > 20160)
				{
					return 15;
				}
				return minutesBeforeStart;
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			CalendarItemBase calendarItemBase = (CalendarItemBase)base.XsoItem;
			calendarItemBase[ItemSchema.ReminderIsSet] = true;
			calendarItemBase[base.PropertyDef] = ((IIntegerProperty)srcProperty).IntegerData;
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			CalendarItemBase calendarItemBase = (CalendarItemBase)base.XsoItem;
			if (!(calendarItemBase.TryGetProperty(base.PropertyDef) is PropertyError))
			{
				calendarItemBase[ItemSchema.ReminderIsSet] = false;
			}
		}
	}
}
