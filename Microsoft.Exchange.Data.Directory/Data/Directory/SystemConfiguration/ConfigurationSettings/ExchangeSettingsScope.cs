using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public enum ExchangeSettingsScope
	{
		Forest = 100,
		Dag = 150,
		Server = 200,
		Process = 250,
		Database = 300,
		Organization = 400,
		User = 500,
		Generic = 600
	}
}
