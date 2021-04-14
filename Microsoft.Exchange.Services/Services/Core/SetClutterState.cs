using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Clutter;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SetClutterState : SingleStepServiceCommand<SetClutterStateRequest, SetClutterStateResponse>
	{
		public SetClutterState(CallContext callContext, SetClutterStateRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return base.Result.Value;
		}

		internal override ServiceResult<SetClutterStateResponse> Execute()
		{
			MailboxSession mailboxSession = base.GetMailboxSession(base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
			switch (base.Request.Command)
			{
			case SetClutterStateCommand.EnableClutter:
				ClutterUtilities.OptUserIn(mailboxSession);
				break;
			case SetClutterStateCommand.DisableClutter:
				ClutterUtilities.OptUserOut(mailboxSession);
				break;
			default:
				throw new ArgumentException("Unsupported clutter request command: {0}".FormatWith(new object[]
				{
					base.Request.Command
				}));
			}
			SetClutterStateResponse value = new SetClutterStateResponse(ServiceResultCode.Success, null)
			{
				ClutterState = Util.GetMailboxClutterState(mailboxSession)
			};
			return new ServiceResult<SetClutterStateResponse>(value);
		}
	}
}
