using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetCalendarGroupCommand : EntityContainersCommand<GetCalendarGroupRequest, GetCalendarGroupResponse>
	{
		public GetCalendarGroupCommand(GetCalendarGroupRequest request) : base(request)
		{
		}

		protected override GetCalendarGroupResponse InternalExecute()
		{
			CalendarGroup dataEntityCalendarGroup = this.EntityContainers.Calendaring.CalendarGroups.Read(EwsIdConverter.ODataIdToEwsId(base.Request.Id), base.CreateCommandContext(null));
			return new GetCalendarGroupResponse(base.Request)
			{
				Result = GetCalendarGroupCommand.DataEntityCalendarGroupToEntity(dataEntityCalendarGroup, base.Request.ODataQueryOptions)
			};
		}

		public static CalendarGroup DataEntityCalendarGroupToEntity(CalendarGroup dataEntityCalendarGroup, ODataQueryOptions odataQueryOptions)
		{
			ArgumentValidator.ThrowIfNull("dataEntityCalendarGroup", dataEntityCalendarGroup);
			ArgumentValidator.ThrowIfNull("odataQueryOptions", odataQueryOptions);
			CalendarGroup calendarGroup = DataEntityObjectFactory.CreateEntity<CalendarGroup>(dataEntityCalendarGroup);
			QueryAdapter queryAdapter = new DataEntityQueryAdpater(CalendarGroupSchema.SchemaInstance, odataQueryOptions);
			foreach (PropertyDefinition propertyDefinition in queryAdapter.RequestedProperties)
			{
				propertyDefinition.DataEntityPropertyProvider.GetPropertyFromDataSource(calendarGroup, propertyDefinition, dataEntityCalendarGroup);
			}
			return calendarGroup;
		}
	}
}
