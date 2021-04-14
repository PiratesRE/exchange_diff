using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UpdateCalendarGroupCommand : EntityContainersCommand<UpdateCalendarGroupRequest, UpdateCalendarGroupResponse>
	{
		public UpdateCalendarGroupCommand(UpdateCalendarGroupRequest request) : base(request)
		{
		}

		protected override UpdateCalendarGroupResponse InternalExecute()
		{
			CalendarGroup entity = DataEntityObjectFactory.CreateAndSetPropertiesOnDataEntityForUpdate<CalendarGroup>(base.Request.Change);
			string key = EwsIdConverter.ODataIdToEwsId(base.Request.Id);
			CalendarGroup dataEntityCalendarGroup = this.EntityContainers.Calendaring.CalendarGroups.Update(key, entity, base.CreateCommandContext(null));
			return new UpdateCalendarGroupResponse(base.Request)
			{
				Result = GetCalendarGroupCommand.DataEntityCalendarGroupToEntity(dataEntityCalendarGroup, base.Request.ODataQueryOptions)
			};
		}
	}
}
