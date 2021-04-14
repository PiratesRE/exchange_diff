using System;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal enum ExpirationLimit
	{
		RunspaceRefresh,
		ExternalAccountRunspaceTermination,
		MaxValue = 1
	}
}
