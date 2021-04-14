using System;

namespace Microsoft.Exchange.Configuration.FailFast
{
	internal enum BlockedReason
	{
		None,
		BySelf,
		ByTenant,
		ByServer
	}
}
