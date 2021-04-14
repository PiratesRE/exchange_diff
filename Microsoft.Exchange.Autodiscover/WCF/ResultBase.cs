using System;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal abstract class ResultBase
	{
		internal ResultBase(UserResultMapping userResultMapping)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<ResultBase, string>((long)this.GetHashCode(), "{0} constructor called for '{1}'.", this, userResultMapping.Mailbox);
			this.userResultMapping = userResultMapping;
		}

		internal UserResultMapping UserResultMapping
		{
			get
			{
				return this.userResultMapping;
			}
		}

		internal abstract UserResponse CreateResponse(IBudget budget);

		protected static UserResponse GenerateUserResponseError(UserConfigurationSettings settings, UserSettingErrorCollection settingErrors)
		{
			UserResponse userResponse = new UserResponse();
			switch (settings.ErrorCode)
			{
			case UserConfigurationSettingsErrorCode.RedirectAddress:
				userResponse.ErrorCode = ErrorCode.RedirectAddress;
				userResponse.ErrorMessage = settings.ErrorMessage;
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.SetRedirectionType(RedirectionType.EmailAddressRedirect);
				userResponse.RedirectTarget = settings.RedirectTarget;
				return userResponse;
			case UserConfigurationSettingsErrorCode.RedirectUrl:
				userResponse.ErrorCode = ErrorCode.RedirectUrl;
				userResponse.ErrorMessage = settings.ErrorMessage;
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.SetRedirectionType(RedirectionType.UrlRedirect);
				userResponse.RedirectTarget = settings.RedirectTarget;
				return userResponse;
			case UserConfigurationSettingsErrorCode.InvalidUser:
				userResponse.ErrorCode = ErrorCode.InvalidUser;
				userResponse.ErrorMessage = settings.ErrorMessage;
				return userResponse;
			case UserConfigurationSettingsErrorCode.InternalServerError:
				userResponse.ErrorCode = ErrorCode.InternalServerError;
				userResponse.ErrorMessage = settings.ErrorMessage;
				return userResponse;
			}
			userResponse.UserSettingErrors = settingErrors;
			userResponse.ErrorCode = ErrorCode.InvalidRequest;
			userResponse.ErrorMessage = (string.IsNullOrEmpty(settings.ErrorMessage) ? Strings.InvalidRequest : settings.ErrorMessage);
			return userResponse;
		}

		protected UserResponse CreateInvalidUserResponse()
		{
			return ResultBase.GenerateUserResponseError(new UserConfigurationSettings
			{
				ErrorCode = UserConfigurationSettingsErrorCode.InvalidUser,
				ErrorMessage = string.Format(Strings.InvalidUser, this.userResultMapping.Mailbox)
			}, this.userResultMapping.CallContext.SettingErrors);
		}

		protected UserResultMapping userResultMapping;
	}
}
