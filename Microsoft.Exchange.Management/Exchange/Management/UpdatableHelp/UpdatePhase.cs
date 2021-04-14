using System;

namespace Microsoft.Exchange.Management.UpdatableHelp
{
	public enum UpdatePhase
	{
		Checking,
		Downloading,
		Extracting,
		Validating,
		Installing,
		Finalizing,
		Rollback
	}
}
