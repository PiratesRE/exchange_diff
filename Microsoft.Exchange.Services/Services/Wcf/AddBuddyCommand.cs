using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class AddBuddyCommand : ServiceCommand<bool>
	{
		public AddBuddyCommand(CallContext callContext, Buddy buddy) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(buddy, "buddy", "AddBuddy::AddBuddy");
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
			this.buddy = buddy;
		}

		protected override bool InternalExecute()
		{
			new AddBuddy(this.session, this.buddy).Execute();
			return true;
		}

		private readonly MailboxSession session;

		private readonly Buddy buddy;
	}
}
