using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	internal enum SharedTenantConfigurationState : long
	{
		UnSupported = 0L,
		NotShared = 1L,
		Shared = 2L,
		Static = 4L,
		Dehydrated = 8L
	}
}
