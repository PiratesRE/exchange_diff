using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.Rpc;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetUMPin : SingleStepServiceCommand<GetUMPinRequest, GetUMPinResponseMessage>
	{
		public GetUMPin(CallContext callContext, GetUMPinRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetUMPinResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<GetUMPinResponseMessage> Execute()
		{
			IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			ADUser user = adrecipientSession.FindADUserByObjectId(base.CallContext.AccessingADUser.ObjectId);
			PINInfo umpin;
			using (XSOUMUserMailboxAccessor xsoumuserMailboxAccessor = new XSOUMUserMailboxAccessor(user, base.MailboxIdentityMailboxSession))
			{
				umpin = xsoumuserMailboxAccessor.GetUMPin();
			}
			GetUMPinResponseMessage value = new GetUMPinResponseMessage
			{
				PinInfo = umpin
			};
			return new ServiceResult<GetUMPinResponseMessage>(value);
		}
	}
}
