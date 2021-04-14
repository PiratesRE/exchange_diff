using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IToDoItem : IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		string Subject { get; set; }

		string InternetMessageId { get; }

		Reminders<ModernReminder> ModernReminders { get; set; }

		RemindersState<ModernReminderState> ModernRemindersState { get; set; }

		GlobalObjectId GetGlobalObjectId();
	}
}
