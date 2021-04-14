using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConfigProvider : IDisposable, IDiagnosable
	{
		DateTime LastUpdated { get; }

		bool IsInitialized { get; }

		void Initialize();

		T GetConfig<T>(string settingName);

		T GetConfig<T>(ISettingsContext context, string settingName);

		T GetConfig<T>(ISettingsContext context, string settingName, T defaultValue);

		bool TryGetConfig<T>(ISettingsContext context, string settingName, out T settingValue);
	}
}
