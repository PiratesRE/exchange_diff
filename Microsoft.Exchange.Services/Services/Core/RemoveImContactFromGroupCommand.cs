using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class RemoveImContactFromGroupCommand : SingleStepServiceCommand<RemoveImContactFromGroupRequest, ServiceResultNone>
	{
		public RemoveImContactFromGroupCommand(CallContext callContext, RemoveImContactFromGroupRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new RemoveImContactFromGroupResponseMessage(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			IdAndSession idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(base.Request.ContactId);
			IdAndSession idAndSession2 = base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(base.Request.GroupId);
			StoreId id = idAndSession.Id;
			StoreId id2 = idAndSession2.Id;
			MailboxSession mailboxSession = (idAndSession.Session as MailboxSession) ?? base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			new RemoveImContactFromGroup(mailboxSession, id, id2, new XSOFactory()).Execute();
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}
	}
}
