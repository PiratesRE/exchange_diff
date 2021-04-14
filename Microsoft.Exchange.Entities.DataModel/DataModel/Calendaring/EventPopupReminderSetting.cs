using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public sealed class EventPopupReminderSetting : StorageEntity<EventPopupReminderSettingSchema>
	{
		public bool IsReminderSet
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.IsReminderSetProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.IsReminderSetProperty, value);
			}
		}

		public int ReminderMinutesBeforeStart
		{
			get
			{
				return base.GetPropertyValueOrDefault<int>(base.Schema.ReminderMinutesBeforeStartProperty);
			}
			set
			{
				base.SetPropertyValue<int>(base.Schema.ReminderMinutesBeforeStartProperty, value);
			}
		}
	}
}
