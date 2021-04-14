using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ModernRemindersMessageProperty : ModernRemindersPropertyBase
	{
		private ModernRemindersMessageProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static ModernRemindersMessageProperty CreateCommand(CommandContext commandContext)
		{
			return new ModernRemindersMessageProperty(commandContext);
		}

		internal static void SetModernRemindersMessageProperty(ServiceObject serviceObject, Item item)
		{
			ModernReminderType[] array = (ModernReminderType[])serviceObject.PropertyBag[MessageSchema.ModernReminders];
			if (array != null)
			{
				item.Load(new PropertyDefinition[]
				{
					MessageItemSchema.QuickCaptureReminders
				});
				Reminders<ModernReminder> reminders = new Reminders<ModernReminder>();
				ModernRemindersPropertyBase.GetModernReminderSettings(array, reminders);
				((IToDoItem)item).ModernReminders = reminders;
			}
		}

		internal override void SetProperty(ServiceObject serviceObject, Item item)
		{
			ModernRemindersMessageProperty.SetModernRemindersMessageProperty(serviceObject, item);
		}
	}
}
