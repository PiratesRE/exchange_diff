using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.CalendarCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ReadCalendar : ReadEntityCommand<Calendars, Calendar>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.ReadCalendarTracer;
			}
		}

		protected override Calendar OnExecute()
		{
			StoreId id = this.Scope.IdConverter.ToStoreObjectId(base.EntityKey);
			return this.Scope.CalendarGroupEntryDataProvider.Read(id);
		}
	}
}
