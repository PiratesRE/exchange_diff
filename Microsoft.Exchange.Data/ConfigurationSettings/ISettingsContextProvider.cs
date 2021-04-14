using System;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	internal interface ISettingsContextProvider
	{
		ISettingsContext GetSettingsContext();
	}
}
