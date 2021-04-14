using System;

namespace System.Deployment.Internal.Isolation
{
	internal enum StoreTransactionOperationType
	{
		Invalid,
		SetCanonicalizationContext = 14,
		StageComponent = 20,
		PinDeployment,
		UnpinDeployment,
		StageComponentFile,
		InstallDeployment,
		UninstallDeployment,
		SetDeploymentMetadata,
		Scavenge
	}
}
