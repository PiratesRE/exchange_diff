using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Flags]
	internal enum MigrationOccupantType
	{
		Regular = 0,
		DryRun = 1,
		Test = 2
	}
}
