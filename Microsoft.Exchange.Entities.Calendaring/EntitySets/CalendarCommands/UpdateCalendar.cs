using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.CalendarCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UpdateCalendar : UpdateStorageEntityCommand<Calendars, Calendar>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.UpdateCalendarTracer;
			}
		}

		protected override Calendar OnExecute()
		{
			CalendarGroupEntryDataProvider calendarGroupEntryDataProvider = this.Scope.CalendarGroupEntryDataProvider;
			Calendar result;
			using (ICalendarGroupEntry calendarGroupEntry = calendarGroupEntryDataProvider.ValidateAndBindToWrite(base.Entity))
			{
				if (calendarGroupEntry.IsLocalMailboxCalendar)
				{
					this.Scope.CalendarFolderDataProvider.UpdateOnly(base.Entity, calendarGroupEntry.CalendarId);
				}
				result = calendarGroupEntryDataProvider.Update(base.Entity, calendarGroupEntry, this.Context);
			}
			return result;
		}
	}
}
