using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateCalendarCommand : EntityContainersCommand<CreateCalendarRequest, CreateCalendarResponse>
	{
		public CreateCalendarCommand(CreateCalendarRequest request) : base(request)
		{
		}

		protected override CreateCalendarResponse InternalExecute()
		{
			Calendar entity = DataEntityObjectFactory.CreateAndSetPropertiesOnDataEntityForCreate<Calendar>(base.Request.Template);
			Calendar dataEntityCalendar;
			if (base.Request.CalendarGroupId == null)
			{
				dataEntityCalendar = this.EntityContainers.Calendaring.Calendars.Create(entity, base.CreateCommandContext(null));
			}
			else
			{
				string calendarGroupId = EwsIdConverter.ODataIdToEwsId(base.Request.CalendarGroupId);
				dataEntityCalendar = this.EntityContainers.Calendaring.CalendarGroups[calendarGroupId].Calendars.Create(entity, base.CreateCommandContext(null));
			}
			return new CreateCalendarResponse(base.Request)
			{
				Result = GetCalendarCommand.DataEntityCalendarToEntity(dataEntityCalendar, base.Request.ODataQueryOptions)
			};
		}
	}
}
