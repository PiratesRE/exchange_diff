using System;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[Flags]
	[Serializable]
	public enum ConfigFlags
	{
		None = 0,
		DisallowADConfig = 1,
		DisallowAppConfig = 2,
		LowADConfigPriority = 4
	}
}
