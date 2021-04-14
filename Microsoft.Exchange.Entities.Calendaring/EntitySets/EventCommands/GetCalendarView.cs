using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.EntitySets.Commands;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetCalendarView : EntityCommand<Events, IEnumerable<Event>>
	{
		public GetCalendarView(ICalendarViewParameters parameters)
		{
			this.Parameters = parameters;
		}

		public ICalendarViewParameters Parameters { get; private set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.GetCalendarViewTracer;
			}
		}

		protected override IEnumerable<Event> OnExecute()
		{
			EventDataProvider eventDataProvider = this.Scope.EventDataProvider;
			IEnumerable<PropertyDefinition> first = eventDataProvider.MapProperties(FindEvents.HardCodedProperties);
			PropertyDefinition[] array = first.Concat(FindEvents.HardCodedPropertyDependencies).ToArray<PropertyDefinition>();
			bool includeSeriesMasters;
			if (this.Context == null || !this.Context.TryGetCustomParameter<bool>("ReturnSeriesMaster", out includeSeriesMasters))
			{
				includeSeriesMasters = false;
			}
			bool flag = this.Context != null && this.Context.RequestedProperties != null && FindEvents.NeedsReread(this.Context.RequestedProperties, this.Trace);
			IEnumerable<Event> calendarView = eventDataProvider.GetCalendarView(this.Parameters.EffectiveStartTime, this.Parameters.EffectiveEndTime, includeSeriesMasters, flag ? this.Scope.EventDataProvider.MapProperties(FindEvents.IdProperty).ToArray<PropertyDefinition>() : array);
			if (flag)
			{
				int count = (this.Context != null) ? this.Context.PageSizeOnReread : 20;
				return (from x in calendarView.Take(count)
				select this.Scope.Read(x.Id, this.Context)).ToList<Event>();
			}
			EventTimeAdjuster adjuster = this.Scope.TimeAdjuster;
			ExTimeZone sessionTimeZone = this.Scope.Session.ExTimeZone;
			return from e in calendarView
			select adjuster.AdjustTimeProperties(e, sessionTimeZone);
		}

		public const string ReturnSeriesMastersParameter = "ReturnSeriesMaster";
	}
}
