using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class ResetUMMailbox : SingleStepServiceCommand<ResetUMMailboxRequest, ResetUMMailboxResponseMessage>
	{
		public ResetUMMailbox(CallContext callContext, ResetUMMailboxRequest request) : base(callContext, request)
		{
			this.keepProperties = request.KeepProperties;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new ResetUMMailboxResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<ResetUMMailboxResponseMessage> Execute()
		{
			IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			ADUser user = adrecipientSession.FindADUserByObjectId(base.CallContext.AccessingADUser.ObjectId);
			using (XSOUMUserMailboxAccessor xsoumuserMailboxAccessor = new XSOUMUserMailboxAccessor(user, base.MailboxIdentityMailboxSession))
			{
				xsoumuserMailboxAccessor.ResetUMMailbox(this.keepProperties);
			}
			return new ServiceResult<ResetUMMailboxResponseMessage>(new ResetUMMailboxResponseMessage());
		}

		private readonly bool keepProperties;
	}
}
