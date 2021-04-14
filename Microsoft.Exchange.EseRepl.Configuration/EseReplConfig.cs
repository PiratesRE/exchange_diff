using System;
using Microsoft.Exchange.EseRepl.Common;

namespace Microsoft.Exchange.EseRepl.Configuration
{
	internal class EseReplConfig : IEseReplConfig
	{
		private EseReplConfig()
		{
		}

		public static EseReplConfig Instance
		{
			get
			{
				return EseReplConfig.instance;
			}
		}

		public string RegistryRootKeyName
		{
			get
			{
				return this.registryRootKeyName;
			}
		}

		private static EseReplConfig instance = new EseReplConfig();

		private readonly string registryRootKeyName = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\Replay\\Parameters", "v15");
	}
}
