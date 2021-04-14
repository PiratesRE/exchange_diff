using System;

namespace Microsoft.Exchange.Setup.Parser
{
	[Flags]
	public enum SetupOperations
	{
		None = 0,
		Install = 1,
		Uninstall = 2,
		AllUIInstallations = 11,
		RecoverServer = 4,
		Upgrade = 8,
		AllModeOperations = 15,
		PrepareAD = 256,
		PrepareSchema = 512,
		PrepareDomain = 2048,
		LanguagePack = 262144,
		AllClientAndServerLanguagePackOperations = 262144,
		AddUmLanguagePack = 16384,
		RemoveUmLanguagePack = 32768,
		AllUmLanguagePackOperations = 49152,
		AllPrepareOperations = 92928,
		AllSetupOperations = 387855,
		AllMSIInstallOperations = 13,
		NewProvisionedServer = 8192,
		RemoveProvisionedServer = 16384,
		PrepareSCT = 65536
	}
}
