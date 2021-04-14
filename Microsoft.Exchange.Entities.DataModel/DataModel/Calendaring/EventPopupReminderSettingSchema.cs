using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public sealed class EventPopupReminderSettingSchema : StorageEntitySchema
	{
		public EventPopupReminderSettingSchema()
		{
			base.RegisterPropertyDefinition(EventPopupReminderSettingSchema.StaticIsReminderSetProperty);
			base.RegisterPropertyDefinition(EventPopupReminderSettingSchema.StaticReminderMinutesBeforeStartProperty);
		}

		public TypedPropertyDefinition<bool> IsReminderSetProperty
		{
			get
			{
				return EventPopupReminderSettingSchema.StaticIsReminderSetProperty;
			}
		}

		public TypedPropertyDefinition<int> ReminderMinutesBeforeStartProperty
		{
			get
			{
				return EventPopupReminderSettingSchema.StaticReminderMinutesBeforeStartProperty;
			}
		}

		private static readonly TypedPropertyDefinition<bool> StaticIsReminderSetProperty = new TypedPropertyDefinition<bool>("EventPopupReminderSetting.IsReminderSet", false, true);

		private static readonly TypedPropertyDefinition<int> StaticReminderMinutesBeforeStartProperty = new TypedPropertyDefinition<int>("EventPopupReminderSetting.ReminderMinutesBeforeStart", 0, true);
	}
}
