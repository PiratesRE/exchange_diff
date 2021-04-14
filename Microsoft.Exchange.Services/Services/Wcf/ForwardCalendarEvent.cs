using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ForwardCalendarEvent : SingleStepServiceCommand<ForwardCalendarEventRequest, VoidResult>
	{
		public ForwardCalendarEvent(CallContext callContext, ForwardCalendarEventRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new ForwardCalendarEventResponse(base.Result);
		}

		internal override ServiceResult<VoidResult> Execute()
		{
			EntitiesHelper entitiesHelper = new EntitiesHelper(base.CallContext);
			StoreSession session;
			IEvents events = entitiesHelper.GetEvents(base.Request.CalendarId.BaseFolderId, out session);
			Microsoft.Exchange.Services.Core.Types.ItemId eventId2 = base.Request.EventId;
			entitiesHelper.Execute(delegate(string eventId, CommandContext context)
			{
				events.Forward(eventId, this.Request.Parameters, context);
			}, session, BasicTypes.Item, eventId2.Id, eventId2.ChangeKey);
			return new ServiceResult<VoidResult>(VoidResult.Value);
		}
	}
}
