using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	[Serializable]
	internal class EntityReminderOffsetProperty : EntityProperty, IReminder160Property, IIntegerProperty, IProperty
	{
		public EntityReminderOffsetProperty() : base(SchematizedObject<EventSchema>.SchemaInstance.PopupReminderSettingsProperty, PropertyType.ReadWrite, false)
		{
		}

		public bool ReminderIsSet
		{
			get
			{
				return base.CalendaringEvent.PopupReminderSettings.Count > 0 && base.CalendaringEvent.PopupReminderSettings[0].IsReminderSet;
			}
		}

		public virtual int IntegerData
		{
			get
			{
				if (this.ReminderIsSet)
				{
					return base.CalendaringEvent.PopupReminderSettings[0].ReminderMinutesBeforeStart;
				}
				return -1;
			}
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			IReminder160Property reminder160Property = srcProperty as IReminder160Property;
			if (reminder160Property != null)
			{
				if (base.CalendaringEvent.PopupReminderSettings == null)
				{
					base.CalendaringEvent.PopupReminderSettings = new List<EventPopupReminderSetting>(1);
				}
				if (base.CalendaringEvent.PopupReminderSettings.Count == 0)
				{
					base.CalendaringEvent.PopupReminderSettings.Add(new EventPopupReminderSetting());
				}
				base.CalendaringEvent.PopupReminderSettings[0].IsReminderSet = reminder160Property.ReminderIsSet;
				if (reminder160Property.ReminderIsSet)
				{
					base.CalendaringEvent.PopupReminderSettings[0].ReminderMinutesBeforeStart = reminder160Property.IntegerData;
				}
			}
		}
	}
}
