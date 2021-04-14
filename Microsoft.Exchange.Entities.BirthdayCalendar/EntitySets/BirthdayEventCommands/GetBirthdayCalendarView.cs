using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Entities.EntitySets.Commands;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets.BirthdayEventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetBirthdayCalendarView : EntityCommand<IBirthdayEvents, IEnumerable<BirthdayEvent>>
	{
		public GetBirthdayCalendarView(IBirthdayEvents scope, ExDateTime rangeStart, ExDateTime rangeEnd)
		{
			if (rangeStart > rangeEnd)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Range start ({0}) must not be greater than range end ({1})", new object[]
				{
					rangeStart,
					rangeEnd
				}), "rangeStart");
			}
			this.Scope = scope;
			this.rangeStart = rangeStart;
			this.rangeEnd = rangeEnd;
		}

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.BirthdayEventDataProviderTracer;
			}
		}

		protected override IEnumerable<BirthdayEvent> OnExecute()
		{
			return this.Scope.BirthdayEventDataProvider.GetBirthdayCalendarView(this.rangeStart, this.rangeEnd);
		}

		private readonly ExDateTime rangeStart;

		private readonly ExDateTime rangeEnd;
	}
}
