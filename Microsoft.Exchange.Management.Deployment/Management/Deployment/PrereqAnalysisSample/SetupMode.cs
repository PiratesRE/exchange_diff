using System;

namespace Microsoft.Exchange.Management.Deployment.PrereqAnalysisSample
{
	[Flags]
	public enum SetupMode
	{
		None = 0,
		Install = 1,
		Upgrade = 2,
		Uninstall = 4,
		DisasterRecovery = 8,
		All = 15
	}
}
