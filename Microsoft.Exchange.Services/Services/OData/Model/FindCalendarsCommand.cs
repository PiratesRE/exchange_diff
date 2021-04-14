using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindCalendarsCommand : EntityContainersCommand<FindCalendarsRequest, FindCalendarsResponse>
	{
		public FindCalendarsCommand(FindCalendarsRequest request) : base(request)
		{
		}

		protected override FindCalendarsResponse InternalExecute()
		{
			DataEntityQueryAdpater dataEntityQueryAdpater = new DataEntityQueryAdpater(CalendarSchema.SchemaInstance, base.Request.ODataQueryOptions);
			IEnumerable<Calendar> source;
			if (string.IsNullOrEmpty(base.Request.CalendarGroupId))
			{
				source = this.EntityContainers.Calendaring.Calendars.Find(dataEntityQueryAdpater.GetEntityQueryOptions(), base.CreateCommandContext(null));
			}
			else
			{
				string calendarGroupId = EwsIdConverter.ODataIdToEwsId(base.Request.CalendarGroupId);
				source = this.EntityContainers.Calendaring.CalendarGroups[calendarGroupId].Calendars.Find(dataEntityQueryAdpater.GetEntityQueryOptions(), base.CreateCommandContext(null));
			}
			IEnumerable<Calendar> entities = (from x in source
			select GetCalendarCommand.DataEntityCalendarToEntity(x, base.Request.ODataQueryOptions)).ToList<Calendar>();
			return new FindCalendarsResponse(base.Request)
			{
				Result = new FindEntitiesResult<Calendar>(entities, -1)
			};
		}
	}
}
