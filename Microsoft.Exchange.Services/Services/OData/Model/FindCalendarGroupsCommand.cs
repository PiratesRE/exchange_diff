using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindCalendarGroupsCommand : EntityContainersCommand<FindCalendarGroupsRequest, FindCalendarGroupsResponse>
	{
		public FindCalendarGroupsCommand(FindCalendarGroupsRequest request) : base(request)
		{
		}

		protected override FindCalendarGroupsResponse InternalExecute()
		{
			DataEntityQueryAdpater dataEntityQueryAdpater = new DataEntityQueryAdpater(CalendarGroupSchema.SchemaInstance, base.Request.ODataQueryOptions);
			IEnumerable<CalendarGroup> source = this.EntityContainers.Calendaring.CalendarGroups.Find(dataEntityQueryAdpater.GetEntityQueryOptions(), base.CreateCommandContext(null));
			IEnumerable<CalendarGroup> entities = (from x in source
			select GetCalendarGroupCommand.DataEntityCalendarGroupToEntity(x, base.Request.ODataQueryOptions)).ToList<CalendarGroup>();
			return new FindCalendarGroupsResponse(base.Request)
			{
				Result = new FindEntitiesResult<CalendarGroup>(entities, -1)
			};
		}
	}
}
