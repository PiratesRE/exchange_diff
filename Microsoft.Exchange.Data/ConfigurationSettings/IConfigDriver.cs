using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConfigDriver : IDisposable
	{
		bool IsInitialized { get; }

		DateTime LastUpdated { get; }

		void Initialize();

		bool TryGetBoxedSetting(ISettingsContext context, string settingName, Type settingType, out object settingValue);

		XElement GetDiagnosticInfo(string argument);
	}
}
