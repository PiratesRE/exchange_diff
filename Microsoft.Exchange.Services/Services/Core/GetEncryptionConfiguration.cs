using System;
using Microsoft.Exchange.Data.ApplicationLogic.E4E;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetEncryptionConfiguration : SingleStepServiceCommand<GetEncryptionConfigurationRequest, GetEncryptionConfigurationResponse>
	{
		public GetEncryptionConfiguration(CallContext callContext, GetEncryptionConfigurationRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetEncryptionConfigurationResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<GetEncryptionConfigurationResponse> Execute()
		{
			ServiceError serviceError = null;
			GetEncryptionConfigurationResponse getEncryptionConfigurationResponse = null;
			try
			{
				MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
				EncryptionConfigurationData encryptionConfigurationData = EncryptionConfigurationHelper.GetEncryptionConfigurationData(mailboxIdentityMailboxSession);
				getEncryptionConfigurationResponse = new GetEncryptionConfigurationResponse();
				getEncryptionConfigurationResponse.ImageBase64 = encryptionConfigurationData.ImageBase64;
				getEncryptionConfigurationResponse.EmailText = encryptionConfigurationData.EmailText;
				getEncryptionConfigurationResponse.PortalText = encryptionConfigurationData.PortalText;
				getEncryptionConfigurationResponse.DisclaimerText = encryptionConfigurationData.DisclaimerText;
				getEncryptionConfigurationResponse.OTPEnabled = encryptionConfigurationData.OTPEnabled;
			}
			catch (Exception e)
			{
				serviceError = EncryptionConfigurationHelper.GetServiceError(e);
			}
			if (serviceError != null)
			{
				return new ServiceResult<GetEncryptionConfigurationResponse>(serviceError);
			}
			return new ServiceResult<GetEncryptionConfigurationResponse>(getEncryptionConfigurationResponse);
		}
	}
}
