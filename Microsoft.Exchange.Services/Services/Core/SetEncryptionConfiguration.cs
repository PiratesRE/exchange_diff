using System;
using Microsoft.Exchange.Data.ApplicationLogic.E4E;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SetEncryptionConfiguration : SingleStepServiceCommand<SetEncryptionConfigurationRequest, ServiceResultNone>
	{
		public SetEncryptionConfiguration(CallContext callContext, SetEncryptionConfigurationRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new SetEncryptionConfigurationResponse(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			ServiceError serviceError = null;
			bool flag = false;
			try
			{
				EncryptionConfigurationData encryptionConfigurationData = new EncryptionConfigurationData(base.Request.ImageBase64, base.Request.EmailText, base.Request.PortalText, base.Request.DisclaimerText, base.Request.OTPEnabled);
				string xml = encryptionConfigurationData.Serialize();
				MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
				flag = EncryptionConfigurationHelper.SetMessageItem(mailboxIdentityMailboxSession, xml);
			}
			catch (Exception e)
			{
				serviceError = EncryptionConfigurationHelper.GetServiceError(e);
			}
			if (!flag && serviceError == null)
			{
				serviceError = new ServiceError("An error occurred in SetEncryptionConfiguration.", ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2012);
			}
			if (serviceError != null)
			{
				return new ServiceResult<ServiceResultNone>(serviceError);
			}
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private const string ErrorMessage = "An error occurred in SetEncryptionConfiguration.";
	}
}
