using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetBirthdayCalendarViewCommand : SingleStepServiceCommand<GetBirthdayCalendarViewRequest, GetBirthdayCalendarViewResponseMessage>
	{
		public GetBirthdayCalendarViewCommand(CallContext callContext, GetBirthdayCalendarViewRequest request) : base(callContext, request)
		{
		}

		private static Microsoft.Exchange.Services.Core.Types.ItemId GetEwsContactIdFrom(IBirthdayEventInternal eventEntity, MailboxSession session)
		{
			return IdConverter.PersonaIdFromStoreId(eventEntity.ContactId, new MailboxId(session));
		}

		private static Microsoft.Exchange.Services.Core.Types.ItemId GetEwsPersonIdFrom(IBirthdayEventInternal eventEntity, IStoreSession session)
		{
			return new Microsoft.Exchange.Services.Core.Types.ItemId(IdConverter.PersonIdToEwsId(session.MailboxGuid, eventEntity.PersonId), null);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetBirthdayCalendarViewResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<GetBirthdayCalendarViewResponseMessage> Execute()
		{
			if (!VariantConfiguration.GetSnapshot(base.CallContext.AccessingADUser.GetContext(null), null, null).OwaServer.XOWABirthdayAssistant.Enabled)
			{
				throw new InvalidOperationException("The user is not on the birthday flight");
			}
			CalendarPageView calendarPageView = new CalendarPageView();
			calendarPageView.StartDateString = base.Request.StartRange;
			calendarPageView.EndDateString = base.Request.EndRange;
			MailboxSession session = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			IEnumerable<Microsoft.Exchange.Entities.DataModel.BirthdayCalendar.BirthdayEvent> birthdayCalendarView = new BirthdaysContainer(session, null).Events.GetBirthdayCalendarView(calendarPageView.StartDateEx, calendarPageView.EndDateEx);
			IEnumerable<Microsoft.Exchange.Services.Wcf.Types.BirthdayEvent> source = from eventEntity in birthdayCalendarView
			select new Microsoft.Exchange.Services.Wcf.Types.BirthdayEvent
			{
				Name = eventEntity.Subject,
				PersonId = GetBirthdayCalendarViewCommand.GetEwsPersonIdFrom(eventEntity, session),
				ContactId = GetBirthdayCalendarViewCommand.GetEwsContactIdFrom(eventEntity, session),
				Birthday = ExDateTimeConverter.ToOffsetXsdDateTime(eventEntity.Birthday.AddMinutes(-eventEntity.Birthday.Bias.TotalMinutes), eventEntity.Birthday.TimeZone),
				Attribution = eventEntity.Attribution,
				IsWritable = eventEntity.IsWritable
			};
			return new ServiceResult<GetBirthdayCalendarViewResponseMessage>(new GetBirthdayCalendarViewResponseMessage
			{
				BirthdayEvents = source.ToArray<Microsoft.Exchange.Services.Wcf.Types.BirthdayEvent>()
			});
		}
	}
}
