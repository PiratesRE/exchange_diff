using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.CalendarGroupCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CreateCalendarGroup : CreateEntityCommand<CalendarGroups, CalendarGroup>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.CreateCalendarGroupCallTracer;
			}
		}

		protected override CalendarGroup OnExecute()
		{
			return this.Scope.CalendarGroupDataProvider.Create(base.Entity);
		}
	}
}
