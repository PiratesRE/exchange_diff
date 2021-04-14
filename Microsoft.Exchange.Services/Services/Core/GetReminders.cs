using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetReminders : SingleStepServiceCommand<GetRemindersRequest, ReminderType[]>
	{
		public GetReminders(CallContext callContext, GetRemindersRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetRemindersResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<ReminderType[]> Execute()
		{
			ExDateTime reminderWindowStart = new ExDateTime(EWSSettings.RequestTimeZone, base.Request.BeginTime);
			if (!string.IsNullOrEmpty(base.Request.BeginTimeString))
			{
				reminderWindowStart = ExDateTime.ParseISO(EWSSettings.RequestTimeZone, base.Request.BeginTimeString);
			}
			ExDateTime exDateTime = new ExDateTime(EWSSettings.RequestTimeZone, base.Request.EndTime);
			if (!string.IsNullOrEmpty(base.Request.EndTimeString))
			{
				exDateTime = ExDateTime.ParseISO(EWSSettings.RequestTimeZone, base.Request.EndTimeString);
			}
			else if (exDateTime <= ExDateTime.MinValue.AddDays(1.0))
			{
				exDateTime = ExDateTime.MaxValue;
			}
			GetRemindersInternal getRemindersInternal = new GetRemindersInternal(base.MailboxIdentityMailboxSession, reminderWindowStart, exDateTime, base.Request.MaxItems, base.Request.ReminderType, base.Request.ReminderGroupType);
			return new ServiceResult<ReminderType[]>(getRemindersInternal.Execute(ExDateTime.Now));
		}
	}
}
