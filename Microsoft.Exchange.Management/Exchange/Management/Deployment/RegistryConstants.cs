using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RegistryConstants
	{
		public const string BaseExchangeKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		public const string LanguageKey = "Language";

		public const string BaseInstallKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\";

		public const string BaseInstallKeyWow6432Node = "SOFTWARE\\Wow6432Node\\Microsoft\\ExchangeServer\\v15\\";

		public const string BaseMsiUninstallKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{4934D1EA-BE46-48B1-8847-F1AF20E892C1}";

		public const string ConfiguredVersion = "ConfiguredVersion";

		public const string UnpackedVersion = "UnpackedVersion";

		public const string UnpackedDatacenterVersion = "UnpackedDatacenterVersion";

		public const string Action = "Action";

		public const string Watermark = "Watermark";

		public const string PostSetupVersion = "PostSetupVersion";

		public const string ServerLanguage = "Server Language";

		public const string InstallSource = "InstallSource";

		public const string TeleLanguagePackKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\TeleLanguagePacks\\";

		public const string TransLanguagePackKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\TransLanguagePacks\\";

		public const string TtsLanguagePackKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\TtsLanguagePacks\\";
	}
}
