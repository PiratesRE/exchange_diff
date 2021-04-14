using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyTranslationRules
{
	internal class EventPopupReminderSettingsRules : ITranslationRule<ICalendarItemBase, IEvent>
	{
		public void FromRightToLeftType(ICalendarItemBase calendarItem, IEvent evt)
		{
			ExAssert.RetailAssert(evt != null, "evt is null");
			ExAssert.RetailAssert(calendarItem != null, "calendarItem is null");
			IList<EventPopupReminderSetting> popupReminderSettings = evt.PopupReminderSettings;
			if (popupReminderSettings != null)
			{
				ExAssert.RetailAssert(popupReminderSettings.Count == 1, "reminderSettings.Count is not 1, actual count is {0}", new object[]
				{
					popupReminderSettings.Count
				});
				EventPopupReminderSetting eventPopupReminderSetting = popupReminderSettings[0];
				calendarItem.IsReminderSet = eventPopupReminderSetting.IsReminderSet;
				calendarItem.ReminderMinutesBeforeStart = eventPopupReminderSetting.ReminderMinutesBeforeStart;
			}
		}

		public void FromLeftToRightType(ICalendarItemBase calendarItem, IEvent evt)
		{
			ExAssert.RetailAssert(evt != null, "evt is null");
			ExAssert.RetailAssert(calendarItem != null, "calendarItem is null");
			EventPopupReminderSetting item = new EventPopupReminderSetting
			{
				Id = EventPopupReminderSettingsRules.GetDefaultPopupReminderSettingId(evt),
				ChangeKey = evt.ChangeKey,
				IsReminderSet = calendarItem.IsReminderSet,
				ReminderMinutesBeforeStart = calendarItem.ReminderMinutesBeforeStart
			};
			evt.PopupReminderSettings = new List<EventPopupReminderSetting>
			{
				item
			};
		}

		internal static string GetDefaultPopupReminderSettingId(IEvent evt)
		{
			return evt.Id + EventPopupReminderSettingsRules.DefaultReminderId.ToString();
		}

		internal static readonly Guid DefaultReminderId = new Guid("5FAAB62C-B27D-4D7D-83C0-E45A3AB83CF9");
	}
}
