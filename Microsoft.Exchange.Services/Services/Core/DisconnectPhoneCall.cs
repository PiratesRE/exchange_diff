using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.ClientAccess;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class DisconnectPhoneCall : SingleStepServiceCommand<DisconnectPhoneCallRequest, DisconnectPhoneCallResponseMessage>
	{
		public DisconnectPhoneCall(CallContext callContext, DisconnectPhoneCallRequest request) : base(callContext, request)
		{
			this.callId = request.CallId;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new DisconnectPhoneCallResponseMessage(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<DisconnectPhoneCallResponseMessage> Execute()
		{
			using (UMClientCommon umclientCommon = new UMClientCommon(base.CallContext.AccessingPrincipal))
			{
				umclientCommon.Disconnect(this.callId.Id);
			}
			return new ServiceResult<DisconnectPhoneCallResponseMessage>(new DisconnectPhoneCallResponseMessage());
		}

		private PhoneCallId callId;
	}
}
