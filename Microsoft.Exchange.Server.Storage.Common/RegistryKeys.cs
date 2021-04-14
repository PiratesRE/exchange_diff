using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class RegistryKeys
	{
		public const string ParametersSystemKey = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem";

		public const string PerSessionLimitsKey = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\MaxObjsPerMapiSession";

		public const string ReplayParametersKey = "Software\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters";

		public const string DatabaseSubKey = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\{1}-{2}";
	}
}
