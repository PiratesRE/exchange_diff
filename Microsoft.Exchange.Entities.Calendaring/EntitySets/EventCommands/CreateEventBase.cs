using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class CreateEventBase : CreateEntityCommand<Events, Event>
	{
		protected virtual void ValidateSeriesId()
		{
			if (base.Entity.IsPropertySet(base.Entity.Schema.SeriesIdProperty))
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorCallerCantSpecifySeriesId);
			}
		}

		protected virtual void ValidateParameters()
		{
			if (base.Entity.IsPropertySet(base.Entity.Schema.SeriesMasterIdProperty))
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorCallerCantSpecifySeriesMasterId);
			}
			this.ValidateSeriesId();
		}
	}
}
