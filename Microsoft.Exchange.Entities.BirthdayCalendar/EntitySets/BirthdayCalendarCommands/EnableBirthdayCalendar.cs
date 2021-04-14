using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets.BirthdayCalendarCommands
{
	internal class EnableBirthdayCalendar : EntityCommand<BirthdayCalendars, BirthdayCalendar>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.EnableBirthdayCalendarTracer;
			}
		}

		protected override BirthdayCalendar OnExecute()
		{
			throw new NotImplementedException();
		}
	}
}
