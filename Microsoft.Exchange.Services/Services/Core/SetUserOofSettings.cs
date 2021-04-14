using System;
using System.Diagnostics;
using System.ServiceModel;
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
	internal sealed class SetUserOofSettings : AvailabilityCommandBase<SetUserOofSettingsRequest, SetUserOofSettingsResponse>
	{
		public SetUserOofSettings(CallContext callContext, SetUserOofSettingsRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			if (base.Result.Value == null)
			{
				return new SetUserOofSettingsResponse
				{
					ResponseMessage = new ResponseMessage(base.Result.Code, base.Result.Error)
				};
			}
			return base.Result.Value;
		}

		internal override ServiceResult<SetUserOofSettingsResponse> Execute()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			PerformanceContext ldapInitialPerformanceContext = new PerformanceContext(PerformanceContext.Current);
			PerformanceContext rpcInitialPerformanceContext;
			NativeMethods.GetTLSPerformanceContext(out rpcInitialPerformanceContext);
			this.EmailAddress = base.Request.Mailbox;
			this.UserOofSettings = base.Request.UserOofSettings;
			ServiceResult<SetUserOofSettingsResponse> result;
			try
			{
				result = this.SetUserOofSettingsExecute();
			}
			finally
			{
				stopwatch.Stop();
				base.LogLatency(ldapInitialPerformanceContext, rpcInitialPerformanceContext);
				base.CallContext.ProtocolLog.AppendGenericInfo("TimeInSetUserOOFSettings", stopwatch.ElapsedMilliseconds);
			}
			return result;
		}

		private ServiceResult<SetUserOofSettingsResponse> SetUserOofSettingsExecute()
		{
			SetUserOofSettingsResponse setUserOofSettingsResponse = new SetUserOofSettingsResponse();
			FaultException ex = null;
			LocalizedException ex2 = null;
			if (this.EmailAddress == null)
			{
				ex = FaultExceptionUtilities.CreateAvailabilityFault(new InvalidParameterException(CoreResources.descInvalidRequest), FaultParty.Sender);
			}
			else
			{
				try
				{
					MailboxSession mailboxSession = base.GetMailboxSession(this.EmailAddress.Address);
					if (mailboxSession == null)
					{
						ex2 = new InvalidParameterException(CoreResources.descInvalidOofRequestPublicFolder);
						ex2.ErrorCode = 240;
					}
					else
					{
						this.UserOofSettings.Save(mailboxSession);
					}
				}
				catch (InvalidScheduledOofDuration invalidScheduledOofDuration)
				{
					ex2 = invalidScheduledOofDuration;
					ex2.ErrorCode = 227;
				}
				catch (InvalidUserOofSettings invalidUserOofSettings)
				{
					ex2 = invalidUserOofSettings;
					ex2.ErrorCode = 240;
				}
				setUserOofSettingsResponse.ResponseMessage = ResponseMessageBuilder.ResponseMessageFromExchangeException(ex2);
			}
			if (ex != null)
			{
				throw ex;
			}
			return new ServiceResult<SetUserOofSettingsResponse>(setUserOofSettingsResponse);
		}

		private EmailAddress EmailAddress { get; set; }

		private UserOofSettings UserOofSettings { get; set; }
	}
}
