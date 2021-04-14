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
	internal class DeleteCalendarEvent : SingleStepServiceCommand<DeleteCalendarEventRequest, VoidResult>
	{
		public DeleteCalendarEvent(CallContext callContext, DeleteCalendarEventRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new DeleteCalendarEventResponse(base.Result);
		}

		internal override ServiceResult<VoidResult> Execute()
		{
			EntitiesHelper entitiesHelper = new EntitiesHelper(base.CallContext);
			StoreSession session;
			IEvents events = entitiesHelper.GetEvents(base.Request.CalendarId.BaseFolderId, out session);
			Microsoft.Exchange.Services.Core.Types.ItemId eventId = base.Request.EventId;
			entitiesHelper.Execute(new Action<string, CommandContext>(events.Delete), session, BasicTypes.Item, eventId.Id, eventId.ChangeKey);
			return new ServiceResult<VoidResult>(VoidResult.Value);
		}
	}
}
