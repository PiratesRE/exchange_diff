using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	public enum GlsTenantFlags
	{
		None = 0,
		Exchange15 = 1,
		Filtering = 2
	}
}
