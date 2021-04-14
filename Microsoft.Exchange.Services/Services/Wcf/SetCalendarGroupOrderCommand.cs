using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetCalendarGroupOrderCommand : ServiceCommand<CalendarActionResponse>
	{
		public SetCalendarGroupOrderCommand(CallContext callContext, string groupToPosition, string beforeGroup) : base(callContext)
		{
			this.command = new SetCalendarGroupOrder(base.MailboxIdentityMailboxSession, groupToPosition, beforeGroup);
		}

		protected override CalendarActionResponse InternalExecute()
		{
			return this.command.Execute();
		}

		private readonly SetCalendarGroupOrder command;
	}
}
