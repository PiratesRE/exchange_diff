using System;

namespace Microsoft.Exchange.Migration
{
	[Flags]
	internal enum MigrationFailureFlags
	{
		None = 0,
		Fatal = 1,
		Corruption = 3
	}
}
