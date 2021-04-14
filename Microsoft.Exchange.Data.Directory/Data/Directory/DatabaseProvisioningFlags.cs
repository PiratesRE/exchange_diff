using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal enum DatabaseProvisioningFlags
	{
		None,
		ReservedFlag,
		IsExcludedFromProvisioning,
		IsSuspendedFromProvisioning = 4,
		IsOutOfService = 8,
		IsExcludedFromInitialProvisioning = 16,
		IsExcludedFromProvisioningBySpaceMonitoring = 32,
		IsExcludedFromProvisioningBySchemaVersionMonitoring = 64
	}
}
