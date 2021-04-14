using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Flags]
	internal enum ApplyMailboxPlanFlags
	{
		None = 0,
		PreservePreviousExplicitlySetValues = 1
	}
}
