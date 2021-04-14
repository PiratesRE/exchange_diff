using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class SettingOverrideIdParameter : ADIdParameter
	{
		public SettingOverrideIdParameter()
		{
		}

		public SettingOverrideIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public SettingOverrideIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public SettingOverrideIdParameter(string identity) : base(identity)
		{
		}

		public SettingOverrideIdParameter(ExchangeSettings exchangeSettings) : base(exchangeSettings.Id)
		{
		}

		public static SettingOverrideIdParameter Parse(string identity)
		{
			return new SettingOverrideIdParameter(identity);
		}
	}
}
