using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class ModernRemindersPropertyBase : ComplexPropertyBase, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public ModernRemindersPropertyBase(CommandContext commandContext) : base(commandContext)
		{
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			Item messageItem = (Item)updateCommandSettings.StoreObject;
			ServiceObject serviceObject = setPropertyUpdate.ServiceObject;
			this.SetProperty(serviceObject, messageItem);
		}

		internal abstract void SetProperty(ServiceObject serviceObject, Item messageItem);

		protected static void GetModernReminderSettings(ModernReminderType[] modernReminderTypes, Reminders<ModernReminder> reminders)
		{
			foreach (ModernReminderType modernReminderType in modernReminderTypes)
			{
				ReminderTimeHint reminderTimeHint;
				Hours hours;
				Priority priority;
				if (ModernRemindersPropertyBase.TryGetStorageModernReminderSettings(modernReminderType, out reminderTimeHint, out hours, out priority))
				{
					ModernReminder item = new ModernReminder
					{
						Identifier = modernReminderType.Id,
						ReminderTimeHint = reminderTimeHint,
						Hours = hours,
						Priority = priority,
						Duration = modernReminderType.Duration,
						ReferenceTime = ExDateTimeConverter.Parse(modernReminderType.ReferenceTimeString),
						CustomReminderTime = ExDateTimeConverter.Parse(modernReminderType.CustomReminderTimeString),
						DueDate = ExDateTimeConverter.Parse(modernReminderType.DueDateString)
					};
					reminders.ReminderList.Add(item);
				}
			}
		}

		private static string ConvertExdateTimeToString(ExDateTime exDateTime)
		{
			return ExDateTimeConverter.ToOffsetXsdDateTime(exDateTime, exDateTime.TimeZone);
		}

		private static bool TryGetStorageModernReminderSettings(ModernReminderType modernReminderType, out ReminderTimeHint reminderTimeHint, out Hours hours, out Priority priority)
		{
			reminderTimeHint = ReminderTimeHint.LaterToday;
			hours = Hours.Any;
			priority = Priority.Normal;
			return ModernRemindersPropertyBase.TryGetStorageReminderTimeHint(modernReminderType, out reminderTimeHint) && ModernRemindersPropertyBase.TryGetStorageHours(modernReminderType, out hours) && ModernRemindersPropertyBase.TryGetStoragePriority(modernReminderType, out priority);
		}

		private static bool TryGetStoragePriority(ModernReminderType modernReminderType, out Priority priority)
		{
			priority = Priority.Normal;
			if (modernReminderType == null)
			{
				return false;
			}
			switch (modernReminderType.Priority)
			{
			case Priority.Low:
				priority = Priority.Low;
				break;
			case Priority.Normal:
				priority = Priority.Normal;
				break;
			case Priority.High:
				priority = Priority.High;
				break;
			default:
				return false;
			}
			return true;
		}

		private static bool TryGetStorageHours(ModernReminderType modernReminderType, out Hours hours)
		{
			hours = Hours.Any;
			if (modernReminderType == null)
			{
				return false;
			}
			switch (modernReminderType.Hours)
			{
			case Hours.Personal:
				hours = Hours.Personal;
				break;
			case Hours.Working:
				hours = Hours.Working;
				break;
			case Hours.Any:
				hours = Hours.Any;
				break;
			default:
				return false;
			}
			return true;
		}

		private static bool TryGetStorageReminderTimeHint(ModernReminderType modernReminderType, out ReminderTimeHint reminderTimeHint)
		{
			reminderTimeHint = ReminderTimeHint.LaterToday;
			if (modernReminderType == null)
			{
				return false;
			}
			switch (modernReminderType.ReminderTimeHint)
			{
			case ReminderTimeHint.LaterToday:
				reminderTimeHint = ReminderTimeHint.LaterToday;
				break;
			case ReminderTimeHint.Tomorrow:
				reminderTimeHint = ReminderTimeHint.Tomorrow;
				break;
			case ReminderTimeHint.TomorrowMorning:
				reminderTimeHint = ReminderTimeHint.TomorrowMorning;
				break;
			case ReminderTimeHint.TomorrowAfternoon:
				reminderTimeHint = ReminderTimeHint.TomorrowAfternoon;
				break;
			case ReminderTimeHint.TomorrowEvening:
				reminderTimeHint = ReminderTimeHint.TomorrowEvening;
				break;
			case ReminderTimeHint.ThisWeekend:
				reminderTimeHint = ReminderTimeHint.ThisWeekend;
				break;
			case ReminderTimeHint.NextWeek:
				reminderTimeHint = ReminderTimeHint.NextWeek;
				break;
			case ReminderTimeHint.NextMonth:
				reminderTimeHint = ReminderTimeHint.NextMonth;
				break;
			case ReminderTimeHint.Someday:
				reminderTimeHint = ReminderTimeHint.Someday;
				break;
			case ReminderTimeHint.Custom:
				reminderTimeHint = ReminderTimeHint.Custom;
				break;
			case ReminderTimeHint.Now:
				reminderTimeHint = ReminderTimeHint.Now;
				break;
			case ReminderTimeHint.InTwoDays:
				reminderTimeHint = ReminderTimeHint.InTwoDays;
				break;
			default:
				return false;
			}
			return true;
		}
	}
}
