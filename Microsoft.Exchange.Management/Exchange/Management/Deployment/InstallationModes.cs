using System;

namespace Microsoft.Exchange.Management.Deployment
{
	public enum InstallationModes
	{
		Unknown,
		Install,
		BuildToBuildUpgrade,
		DisasterRecovery,
		Uninstall,
		PostSetupOnly
	}
}
