using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class GetBuddyListCommand : ServiceCommand<GetBuddyListResponse>
	{
		public GetBuddyListCommand(CallContext callContext) : base(callContext)
		{
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
		}

		protected override GetBuddyListResponse InternalExecute()
		{
			return new GetBuddyList(this.session).Execute();
		}

		private readonly MailboxSession session;
	}
}
