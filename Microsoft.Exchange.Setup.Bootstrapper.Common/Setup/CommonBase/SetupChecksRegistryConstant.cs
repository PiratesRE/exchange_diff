using System;
using System.IO;

namespace Microsoft.Exchange.Setup.CommonBase
{
	public static class SetupChecksRegistryConstant
	{
		public const string RegistryExchangePathKey = "MsiInstallPath";

		public const string RegistryExchangeProductMajorVersionKey = "MsiProductMajor";

		public const string RegistryExchangeProductMinorVersionKey = "MsiProductMinor";

		public const string RegistryExchangeBuildMajorVersionKey = "MsiBuildMajor";

		public const string RegistryExchangeBuildMinorVersionKey = "MsiBuildMinor";

		public const string RegistryMicrosoft = "SOFTWARE\\Microsoft";

		public const string RegistryKeyForLanguagePack = "LanguagePackBundlePath";

		public const string RegistrySetupPath = "Setup";

		public const string RegistrySetupSavePath = "Setup-save";

		public const string HubTransportRoleName = "HubTransportRole";

		public const string ClientAccessRoleName = "ClientAccessRole";

		public const string EdgeRoleName = "Hygiene";

		public const string MailboxRoleName = "MailboxRole";

		public const string UnifiedMessagingRoleName = "UnifiedMessagingRole";

		public const string AdminToolsRoleName = "AdminToolsRole";

		public const string SetupAction = "Action";

		public const string UpgradeAction = "BuildToBuildUpgrade";

		public const string DotNetSetupRegistry = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full";

		public const string DotNetVersionRegistryKey = "version";

		public const string PowerShellRegistry = "SOFTWARE\\Microsoft\\PowerShell\\3\\PowerShellEngine";

		public const string PowerShellVersionRegistryKey = "PowerShellVersion";

		public const string MicrosoftHostedKeyPath = "SOFTWARE\\Microsoft\\ExchangeLabs";

		public const string MicrosoftHostedValueName = "DatacenterMode";

		public const string TreatPreReqErrorsAsWarningsKey = "TreatPreReqErrorsAsWarnings";

		public static readonly string RegistryExchangeServer = Path.Combine("SOFTWARE\\Microsoft", "ExchangeServer");

		public static readonly string RegistryExchangePathE12 = Path.Combine("SOFTWARE\\Microsoft", "Exchange\\v8.0");

		public static readonly string RegistryExchangePathE14 = Path.Combine(SetupChecksRegistryConstant.RegistryExchangeServer, "v14");

		public static readonly string RegistryExchangePath = Path.Combine(SetupChecksRegistryConstant.RegistryExchangeServer, "V15");

		public static readonly string RegistryPathForLanguagePack = Path.Combine(SetupChecksRegistryConstant.RegistryExchangeServer, "Language Packs");

		public static readonly Version DotNetVersion = new Version("4.5.50501");

		public static readonly Version PowershellVersion = new Version("3.0");
	}
}
