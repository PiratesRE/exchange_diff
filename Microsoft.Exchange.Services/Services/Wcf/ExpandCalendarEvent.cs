using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ExpandCalendarEvent : SingleStepServiceCommand<ExpandCalendarEventRequest, ExpandedEvent>
	{
		public ExpandCalendarEvent(CallContext callContext, ExpandCalendarEventRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new ExpandCalendarEventResponse(base.Result);
		}

		internal override ServiceResult<ExpandedEvent> Execute()
		{
			EntitiesHelper entitiesHelper = new EntitiesHelper(base.CallContext);
			StoreSession session;
			IEvents events = entitiesHelper.GetEvents(base.Request.CalendarId.BaseFolderId, out session);
			Microsoft.Exchange.Services.Core.Types.ItemId eventId2 = base.Request.EventId;
			ExpandedEvent value = entitiesHelper.Execute<ExpandedEvent>((string eventId, CommandContext context) => events.Expand(eventId, this.Request.Parameters, context), session, BasicTypes.Item, eventId2.Id, eventId2.ChangeKey);
			return new ServiceResult<ExpandedEvent>(value);
		}
	}
}
