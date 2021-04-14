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
	internal class GetCalendarEvent : MultiStepServiceCommand<GetCalendarEventRequest, Event>
	{
		public GetCalendarEvent(CallContext callContext, GetCalendarEventRequest request) : base(callContext, request)
		{
		}

		internal override int StepCount
		{
			get
			{
				return base.Request.EventIds.Length;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetCalendarEventResponse(base.Results);
		}

		internal override ServiceResult<Event> Execute()
		{
			EntitiesHelper entitiesHelper = new EntitiesHelper(base.CallContext);
			StoreSession session;
			IEvents events = entitiesHelper.GetEvents(base.Request.CalendarId.BaseFolderId, out session);
			Microsoft.Exchange.Services.Core.Types.ItemId itemId = base.Request.EventIds[base.CurrentStep];
			Event value = entitiesHelper.Execute<Event>(new Func<string, CommandContext, Event>(events.Read), session, BasicTypes.Item, itemId.Id, itemId.ChangeKey);
			return new ServiceResult<Event>(value);
		}
	}
}
