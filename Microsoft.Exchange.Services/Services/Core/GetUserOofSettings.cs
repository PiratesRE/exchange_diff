using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Availability;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetUserOofSettings : AvailabilityCommandBase<GetUserOofSettingsRequest, GetUserOofSettingsResponse>
	{
		public GetUserOofSettings(CallContext callContext, GetUserOofSettingsRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			if (base.Result.Value == null)
			{
				return new GetUserOofSettingsResponse
				{
					ResponseMessage = new ResponseMessage(base.Result.Code, base.Result.Error),
					EmitAllowExternalOof = false
				};
			}
			return base.Result.Value;
		}

		internal override ServiceResult<GetUserOofSettingsResponse> Execute()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			PerformanceContext ldapInitialPerformanceContext = new PerformanceContext(PerformanceContext.Current);
			PerformanceContext rpcInitialPerformanceContext;
			NativeMethods.GetTLSPerformanceContext(out rpcInitialPerformanceContext);
			this.EmailAddress = base.Request.Mailbox;
			ServiceResult<GetUserOofSettingsResponse> userOofSettingsExecute;
			try
			{
				userOofSettingsExecute = this.GetUserOofSettingsExecute();
			}
			finally
			{
				base.LogLatency(ldapInitialPerformanceContext, rpcInitialPerformanceContext);
				stopwatch.Stop();
				base.CallContext.ProtocolLog.AppendGenericInfo("TimeInGetUserOOFSettings", stopwatch.ElapsedMilliseconds);
			}
			return userOofSettingsExecute;
		}

		private ServiceResult<GetUserOofSettingsResponse> GetUserOofSettingsExecute()
		{
			LocalizedException ex = null;
			GetUserOofSettingsResponse getUserOofSettingsResponse = new GetUserOofSettingsResponse();
			if (this.EmailAddress == null || this.EmailAddress.Address == null)
			{
				throw FaultExceptionUtilities.CreateAvailabilityFault(new InvalidParameterException(CoreResources.descInvalidRequest), FaultParty.Sender);
			}
			try
			{
				MailboxSession mailboxSession = base.GetMailboxSession(this.EmailAddress.Address);
				if (mailboxSession == null)
				{
					ex = new InvalidParameterException(CoreResources.descInvalidOofRequestPublicFolder);
					ex.ErrorCode = 240;
				}
				else
				{
					getUserOofSettingsResponse.OofSettings = UserOofSettings.GetUserOofSettings(mailboxSession);
					getUserOofSettingsResponse.AllowExternalOof = UserOofSettings.GetUserPolicy(mailboxSession.MailboxOwner).ToString();
				}
			}
			catch (InvalidScheduledOofDuration invalidScheduledOofDuration)
			{
				ex = invalidScheduledOofDuration;
				ex.ErrorCode = 227;
			}
			catch (InvalidUserOofSettings invalidUserOofSettings)
			{
				ex = invalidUserOofSettings;
				ex.ErrorCode = 240;
			}
			getUserOofSettingsResponse.ResponseMessage = ResponseMessageBuilder.ResponseMessageFromExchangeException(ex);
			return new ServiceResult<GetUserOofSettingsResponse>(getUserOofSettingsResponse);
		}

		private EmailAddress EmailAddress { get; set; }
	}
}
