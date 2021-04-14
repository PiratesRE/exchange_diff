using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class ExchangeSettingsIdParameter : ADIdParameter
	{
		public ExchangeSettingsIdParameter()
		{
		}

		public ExchangeSettingsIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ExchangeSettingsIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public ExchangeSettingsIdParameter(string identity) : base(identity)
		{
		}

		public ExchangeSettingsIdParameter(ExchangeSettings exchangeSettings) : base(exchangeSettings.Id)
		{
		}

		public static ExchangeSettingsIdParameter Parse(string identity)
		{
			return new ExchangeSettingsIdParameter(identity);
		}
	}
}
