using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UpdateCalendarCommand : EntityContainersCommand<UpdateCalendarRequest, UpdateCalendarResponse>
	{
		public UpdateCalendarCommand(UpdateCalendarRequest request) : base(request)
		{
		}

		protected override UpdateCalendarResponse InternalExecute()
		{
			Calendar entity = DataEntityObjectFactory.CreateAndSetPropertiesOnDataEntityForUpdate<Calendar>(base.Request.Change);
			string key = EwsIdConverter.ODataIdToEwsId(base.Request.Id);
			Calendar dataEntityCalendar = this.EntityContainers.Calendaring.Calendars.Update(key, entity, base.CreateCommandContext(null));
			return new UpdateCalendarResponse(base.Request)
			{
				Result = GetCalendarCommand.DataEntityCalendarToEntity(dataEntityCalendar, base.Request.ODataQueryOptions)
			};
		}
	}
}
