using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class AddImGroupCommand : SingleStepServiceCommand<AddImGroupRequest, ImGroup>
	{
		public AddImGroupCommand(CallContext callContext, AddImGroupRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new AddImGroupResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<ImGroup> Execute()
		{
			string displayName = base.Request.DisplayName;
			MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			RawImGroup rawImGroup = new AddImGroup(mailboxIdentityMailboxSession, displayName, new XSOFactory(), Global.UnifiedContactStoreConfiguration).Execute();
			return new ServiceResult<ImGroup>(ImGroup.LoadFromRawImGroup(rawImGroup, mailboxIdentityMailboxSession));
		}
	}
}
