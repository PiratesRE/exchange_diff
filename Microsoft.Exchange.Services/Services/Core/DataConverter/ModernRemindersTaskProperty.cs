using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ModernRemindersTaskProperty : ModernRemindersPropertyBase, ISetCommand, IUpdateCommand, IPropertyCommand
	{
		private ModernRemindersTaskProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static ModernRemindersTaskProperty CreateCommand(CommandContext commandContext)
		{
			return new ModernRemindersTaskProperty(commandContext);
		}

		void ISetCommand.Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			Item messageItem = commandSettings.StoreObject as Item;
			this.SetProperty(commandSettings.ServiceObject, messageItem);
		}

		internal static void SetModernRemindersTaskProperty(ServiceObject serviceObject, Item item)
		{
			ModernReminderType[] array = (ModernReminderType[])serviceObject.PropertyBag[TaskSchema.ModernReminders];
			if (array != null)
			{
				item.Load(new PropertyDefinition[]
				{
					TaskSchema.QuickCaptureReminders
				});
				Reminders<ModernReminder> reminders = new Reminders<ModernReminder>();
				ModernRemindersPropertyBase.GetModernReminderSettings(array, reminders);
				((IToDoItem)item).ModernReminders = reminders;
			}
		}

		internal override void SetProperty(ServiceObject serviceObject, Item item)
		{
			ModernRemindersTaskProperty.SetModernRemindersTaskProperty(serviceObject, item);
		}
	}
}
