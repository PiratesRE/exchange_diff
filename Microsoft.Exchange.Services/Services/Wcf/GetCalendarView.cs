using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetCalendarView : SingleStepServiceCommand<GetCalendarViewRequest, Event[]>
	{
		public GetCalendarView(CallContext callContext, GetCalendarViewRequest request) : base(callContext, request)
		{
		}

		internal override ServiceResult<Event[]> Execute()
		{
			EntitiesHelper entitiesHelper = new EntitiesHelper(base.CallContext);
			StoreSession session;
			IEvents events = entitiesHelper.GetEvents(base.Request.CalendarId.BaseFolderId, out session);
			events.AsQueryable(null);
			CalendarViewParameters parameters = new CalendarViewParameters(new ExDateTime?(base.Request.StartRange), new ExDateTime?(base.Request.EndRange));
			CommandContext commandContext = new CommandContext();
			commandContext.SetCustomParameter("ReturnSeriesMaster", base.Request.ReturnMasterItems);
			IEnumerable<Event> calendarView = events.GetCalendarView(parameters, commandContext);
			Event[] value = calendarView.ToArray<Event>();
			entitiesHelper.TransformEntityIdsToEwsIds<Event[]>(value, session);
			return new ServiceResult<Event[]>(value);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetCalendarViewResponse(base.Result);
		}
	}
}
