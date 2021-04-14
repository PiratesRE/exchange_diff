using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetClutterState : SingleStepServiceCommand<GetClutterStateRequest, GetClutterStateResponse>
	{
		public GetClutterState(CallContext callContext, GetClutterStateRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return base.Result.Value;
		}

		internal override ServiceResult<GetClutterStateResponse> Execute()
		{
			MailboxSession mailboxSession = base.GetMailboxSession(base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
			GetClutterStateResponse value = new GetClutterStateResponse(ServiceResultCode.Success, null)
			{
				ClutterState = Util.GetMailboxClutterState(mailboxSession)
			};
			return new ServiceResult<GetClutterStateResponse>(value);
		}
	}
}
