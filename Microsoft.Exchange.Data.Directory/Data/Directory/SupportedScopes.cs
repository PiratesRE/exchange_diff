using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	internal enum SupportedScopes
	{
		Topology = 0,
		Tenant = 1
	}
}
