using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CreateSeries : CreateEventBase
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.CreateSeriesTracer;
			}
		}

		protected override Event OnExecute()
		{
			this.ValidateParameters();
			CreateNewSeries createNewSeries = new CreateNewSeries
			{
				Entity = base.Entity,
				Scope = this.Scope,
				ClientId = base.Entity.ClientId
			};
			return createNewSeries.Execute(this.Context);
		}
	}
}
