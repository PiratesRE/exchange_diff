using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.Owa
{
	internal class OwaConstants
	{
		public const string OwaVersionRegistryName = "OwaVersion";

		internal const string OwaLocalPath = "ClientAccess\\owa\\";

		internal const string DefaultExtensionLocalPathFormat = "\\prem\\{0}\\ext\\def\\";

		internal const string KillBitLocalPathFormat = "\\prem\\{0}\\ext\\killbit\\";

		internal const string KillBitFileName = "killbit.xml";

		internal const string OwaDllBinPath = "ClientAccess\\\\owa\\\\bin\\\\Microsoft.Exchange.Clients.Owa.dll";

		public static readonly string OwaSetupInstallKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";
	}
}
