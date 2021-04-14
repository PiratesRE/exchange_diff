using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateCalendarGroupCommand : EntityContainersCommand<CreateCalendarGroupRequest, CreateCalendarGroupResponse>
	{
		public CreateCalendarGroupCommand(CreateCalendarGroupRequest request) : base(request)
		{
		}

		protected override CreateCalendarGroupResponse InternalExecute()
		{
			CalendarGroup entity = DataEntityObjectFactory.CreateAndSetPropertiesOnDataEntityForCreate<CalendarGroup>(base.Request.Template);
			CalendarGroup dataEntityCalendarGroup = this.EntityContainers.Calendaring.CalendarGroups.Create(entity, base.CreateCommandContext(null));
			return new CreateCalendarGroupResponse(base.Request)
			{
				Result = GetCalendarGroupCommand.DataEntityCalendarGroupToEntity(dataEntityCalendarGroup, base.Request.ODataQueryOptions)
			};
		}
	}
}
