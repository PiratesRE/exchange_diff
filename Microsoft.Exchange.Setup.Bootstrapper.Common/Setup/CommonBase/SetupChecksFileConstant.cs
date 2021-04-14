using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Setup.CommonBase
{
	public static class SetupChecksFileConstant
	{
		public static IList<string> GetSetupRequiredFiles()
		{
			return SetupChecksFileConstant.SetupRequiredFiles;
		}

		public static IList<string> GetExcludedPaths()
		{
			return SetupChecksFileConstant.ExcludedPaths;
		}

		public const string BinaryFileNamePattern = "^(.+)\\.(exe|com|dll)$";

		public const string ExchangeSetupUpdatesPath = "Temp\\ExchangeSetup";

		public const string ExchangeSetupUpdatesMspPath = "Temp\\ExchangeSetup\\MspTemp";

		public const string ExchangeSetupSourcePath = "Setup\\ServerRoles\\Common";

		public const string ExchangeSPSourcePath = "Setup\\ServerRoles\\ClientAccess\\ServicePlans";

		public const string ExchangeMailboxSourcePath = "Setup\\ServerRoles\\Mailbox";

		public const string ServerLanguagePackMSIFileName = "ServerLanguagePack.msi";

		public const string SetupRequiredFilesContainingAssembly = "Microsoft.Exchange.Setup.Bootstrapper.Common.dll";

		public const string SetupExeFileName = "Setup.exe";

		public const string ExSetupExeFileName = "ExSetup.exe";

		public const string ExSetupUIExeFileName = "ExSetupUI.exe";

		public const string SetupUIExeFileName = "SetupUI.exe";

		public const string ExchangeServerMsi = "ExchangeServer.msi";

		public const string BinFolderName = "bin";

		public const int WindowsMajorVersion = 6;

		public const int WindowsMinorVersion = 1;

		public const int WindowsServicePack = 1;

		public const string GetSetupRequiredFilesMethodName = "GetSetupRequiredFiles";

		public const string TempDirName = "Temp";

		public const string DefaultCultureName = "en";

		public const string LanguagePackBundleFileName = "LanguagePackBundle.exe";

		public const string LanguagePackBundlePath = "ExchangeSetupLogs\\ExchangeLanguagePack";

		public const string ResourceBaseName = "Microsoft.Exchange.Setup.Bootstrapper.Common.Strings";

		public const string ResourceFileName = "Microsoft.Exchange.Setup.Bootstrapper.Common.Resources.dll";

		public const int CumulativeUpdateVersion = 23;

		private static readonly List<string> ExcludedPaths = new List<string>
		{
			"\\search\\"
		};

		private static readonly IList<string> SetupRequiredFiles = new List<string>
		{
			"ExSetup.exe",
			"ExSetupUI.exe",
			"Interop.NetFw.dll",
			"LPVersioning.xml",
			"Microsoft.Exchange.CabUtility.dll",
			"Microsoft.Exchange.Common.dll",
			"Microsoft.Exchange.Compliance.dll",
			"Microsoft.Exchange.Configuration.ObjectModel.dll",
			"Microsoft.Exchange.Data.Common.dll",
			"Microsoft.Exchange.Data.Directory.dll",
			"Microsoft.Exchange.Data.Storage.dll",
			"Microsoft.Exchange.Diagnostics.dll",
			"Microsoft.Exchange.Setup.CommonBase.dll",
			"Microsoft.Exchange.HelpProvider.dll",
			"Microsoft.Exchange.Management.dll",
			"Microsoft.Exchange.Management.Deployment.dll",
			"Microsoft.Exchange.Management.RbacDefinition.dll",
			"Microsoft.Exchange.Net.dll",
			"Microsoft.Exchange.Rpc.dll",
			"Microsoft.Exchange.Security.dll",
			"Microsoft.Exchange.Setup.AcquireLanguagePack.dll",
			"Microsoft.Exchange.Setup.Bootstrapper.Common.dll",
			"Microsoft.Exchange.Setup.Common.dll",
			"Microsoft.Exchange.Setup.Parser.dll",
			"Microsoft.Exchange.Setup.SignVerfWrapper.dll",
			"msvcp110.dll",
			"msvcr110.dll",
			"res\\SetupClose.png",
			"res\\SetupError.png",
			"res\\SetupHelp.png",
			"res\\SetupPrint.png",
			"res\\SetupPrint_h.png",
			"res\\SetupWarning.png",
			"res\\ExchangeLogo.png"
		};
	}
}
