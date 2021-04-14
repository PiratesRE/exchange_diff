using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CreateEvent : CreateSingleEventBase
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.CreateEventTracer;
			}
		}

		protected override Event CreateNewEvent()
		{
			return this.Scope.EventDataProvider.Create(base.Entity);
		}
	}
}
