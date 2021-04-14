using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Commands;
using Microsoft.Exchange.Entities.EntitySets.Linq.ExtensionMethods;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.CalendarCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FindCalendars : FindEntitiesCommand<Calendars, Calendar>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.FindCalendarsTracer;
			}
		}

		protected override IEnumerable<Calendar> OnExecute()
		{
			IEnumerable<Calendar> source = this.FindAllCalendars();
			return base.QueryOptions.ApplyTo(source.AsQueryable<Calendar>());
		}

		private IEnumerable<Calendar> FindAllCalendars()
		{
			CommandContext commandContext = new CommandContext();
			commandContext.Expand = new string[]
			{
				"Calendars"
			};
			CalendaringContainer calendaringContainer = new CalendaringContainer(this.Scope);
			IEnumerable<CalendarGroup> source = calendaringContainer.CalendarGroups.AsQueryable(commandContext).AsEnumerable<CalendarGroup>();
			CalendarsInCalendarGroup calendarsInCalendarGroup = this.Scope as CalendarsInCalendarGroup;
			if (calendarsInCalendarGroup != null)
			{
				source = (from c in source
				where c.Id == calendarsInCalendarGroup.CalendarGroup.GetKey()
				select c).ToList<CalendarGroup>();
			}
			return source.SelectMany((CalendarGroup calendarGroup) => calendarGroup.Calendars);
		}
	}
}
