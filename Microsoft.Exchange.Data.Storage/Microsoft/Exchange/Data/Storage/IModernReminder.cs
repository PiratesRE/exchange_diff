using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IModernReminder : IReminder
	{
		ReminderTimeHint ReminderTimeHint { get; set; }

		Hours Hours { get; set; }

		Priority Priority { get; set; }

		int Duration { get; set; }

		ExDateTime ReferenceTime { get; set; }

		ExDateTime CustomReminderTime { get; set; }

		ExDateTime DueDate { get; set; }
	}
}
