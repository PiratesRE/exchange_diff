using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CreateCalendarEvent : MultiStepServiceCommand<CreateCalendarEventRequest, Event>
	{
		public CreateCalendarEvent(CallContext callContext, CreateCalendarEventRequest request) : base(callContext, request)
		{
		}

		internal override int StepCount
		{
			get
			{
				return base.Request.Events.Length;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new CreateCalendarEventResponse(base.Results);
		}

		internal override ServiceResult<Event> Execute()
		{
			EntitiesHelper entitiesHelper = new EntitiesHelper(base.CallContext);
			StoreSession session;
			IEvents events = entitiesHelper.GetEvents(base.Request.CalendarId.BaseFolderId, out session);
			Event input = base.Request.Events[base.CurrentStep];
			Event value = entitiesHelper.Execute<Event, Event>(new Func<Event, CommandContext, Event>(events.Create), session, BasicTypes.Item, input);
			return new ServiceResult<Event>(value);
		}
	}
}
