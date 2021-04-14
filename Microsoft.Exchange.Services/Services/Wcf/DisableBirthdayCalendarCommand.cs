using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class DisableBirthdayCalendarCommand : ServiceCommand<CalendarActionResponse>
	{
		public DisableBirthdayCalendarCommand(CallContext callContext) : base(callContext)
		{
		}

		protected override CalendarActionResponse InternalExecute()
		{
			BirthdayCalendar.DisableBirthdayCalendar(base.MailboxIdentityMailboxSession);
			return new CalendarActionResponse();
		}
	}
}
