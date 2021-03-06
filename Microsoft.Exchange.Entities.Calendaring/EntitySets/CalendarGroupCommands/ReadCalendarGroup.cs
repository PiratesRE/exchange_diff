using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.CalendarGroupCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ReadCalendarGroup : ReadEntityCommand<CalendarGroups, CalendarGroup>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.ReadCalendarGroupTracer;
			}
		}

		protected override CalendarGroup OnExecute()
		{
			return this.Scope.CalendarGroupDataProvider.Read(this.GetEntityStoreId());
		}
	}
}
