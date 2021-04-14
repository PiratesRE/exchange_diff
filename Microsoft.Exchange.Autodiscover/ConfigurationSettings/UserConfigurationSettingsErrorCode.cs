using System;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	public enum UserConfigurationSettingsErrorCode
	{
		NoError,
		RedirectAddress,
		RedirectUrl,
		InvalidUser,
		InvalidSetting,
		InvalidRequest,
		InternalServerError
	}
}
