using System;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal interface IExpiringRunspaceConfiguration
	{
		bool ShouldCloseDueToExpiration();
	}
}
