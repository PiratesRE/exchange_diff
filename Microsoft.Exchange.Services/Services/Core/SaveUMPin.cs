using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.Rpc;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SaveUMPin : SingleStepServiceCommand<SaveUMPinRequest, SaveUMPinResponseMessage>
	{
		public SaveUMPin(CallContext callContext, SaveUMPinRequest request) : base(callContext, request)
		{
			this.pinInfo = request.PinInfo;
			this.userUMMailboxPolicyGuid = request.UserUMMailboxPolicyGuid;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new SaveUMPinResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<SaveUMPinResponseMessage> Execute()
		{
			IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			ADUser aduser = adrecipientSession.FindADUserByObjectId(base.CallContext.AccessingADUser.ObjectId);
			if (aduser == null)
			{
				ServiceError error = new ServiceError(Strings.UMMailboxNotFound(base.CallContext.AccessingADUser.PrimarySmtpAddress.ToString()), ResponseCodeType.ErrorRecipientNotFound, 0, ExchangeVersion.Exchange2012);
				return new ServiceResult<SaveUMPinResponseMessage>(error);
			}
			try
			{
				using (XSOUMUserMailboxAccessor xsoumuserMailboxAccessor = new XSOUMUserMailboxAccessor(aduser, base.MailboxIdentityMailboxSession))
				{
					xsoumuserMailboxAccessor.SaveUMPin(this.pinInfo, this.userUMMailboxPolicyGuid);
				}
			}
			catch (UMMbxPolicyNotFoundException ex)
			{
				ServiceError error2 = new ServiceError(ex.Message, ResponseCodeType.ErrorMailboxConfiguration, 0, ExchangeVersion.Exchange2012);
				return new ServiceResult<SaveUMPinResponseMessage>(error2);
			}
			return new ServiceResult<SaveUMPinResponseMessage>(new SaveUMPinResponseMessage());
		}

		private readonly PINInfo pinInfo;

		private readonly Guid userUMMailboxPolicyGuid;
	}
}
