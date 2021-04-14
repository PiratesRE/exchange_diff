using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RegistryConstants
	{
		public static string SetupKey
		{
			get
			{
				return Path.Combine("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\", "Setup");
			}
		}

		public static string SetupBackupKey
		{
			get
			{
				return Path.Combine("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\", "Setup-save");
			}
		}

		public const string BaseExchangeKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		public const string BaseInstallKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\";

		public const string LanguageBundlePathKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\LanguageBundlePath";

		public const string RegistryPathForLanguagePack = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Language Packs\\";

		public const string RegistryKeyForLanguagePack = "LanguagePackBundlePath";

		public const string UmLanguagePackKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\LanguagePacks\\";

		public const string TeleLanguagePackKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\TeleLanguagePacks\\";

		public const string TransLanguagePackKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\TransLanguagePacks\\";

		public const string TtsLanguagePackKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\TtsLanguagePacks\\";

		public const string BaseMsiUninstallKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{4934D1EA-BE46-48B1-8847-F1AF20E892C1}";

		public const string ProfilesDirectoryKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\ProfileList";

		public const string ShellFoldersSubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders";

		public const string MshRegistryRoot = "\\SOFTWARE\\Microsoft\\PowerShell";

		public const string MshRegistryBak = "\\SOFTWARE\\Microsoft\\MSH-save";

		public const string InstallSource = "InstallSource";

		public const string InstallPath = "MsiInstallPath";

		public const string VersionMajor = "MsiProductMajor";

		public const string VersionMinor = "MsiProductMinor";

		public const string VersionBuild = "MsiBuildMajor";

		public const string VersionRevision = "MsiBuildMinor";

		public const string LanguageBundlePath = "LanguageBundlePath";

		public const string ProfilesDirectory = "ProfilesDirectory";
	}
}
