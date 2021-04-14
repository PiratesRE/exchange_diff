using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Autodiscover;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAutodiscoverService
	{
		Uri Url { get; set; }

		GetUserSettingsResponse GetUserSettings(string userSmtpAddress, params UserSettingName[] userSettingNames);
	}
}
