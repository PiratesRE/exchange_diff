using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class InitUMMailbox : SingleStepServiceCommand<InitUMMailboxRequest, InitUMMailboxResponseMessage>
	{
		public InitUMMailbox(CallContext callContext, InitUMMailboxRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new InitUMMailboxResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<InitUMMailboxResponseMessage> Execute()
		{
			IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			ADUser user = adrecipientSession.FindADUserByObjectId(base.CallContext.AccessingADUser.ObjectId);
			using (XSOUMUserMailboxAccessor xsoumuserMailboxAccessor = new XSOUMUserMailboxAccessor(user, base.MailboxIdentityMailboxSession))
			{
				xsoumuserMailboxAccessor.InitUMMailbox();
			}
			return new ServiceResult<InitUMMailboxResponseMessage>(new InitUMMailboxResponseMessage());
		}
	}
}
