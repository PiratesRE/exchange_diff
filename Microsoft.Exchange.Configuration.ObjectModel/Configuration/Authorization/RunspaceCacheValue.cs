using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class RunspaceCacheValue
	{
		internal RunspaceCacheValue()
		{
		}

		internal CostHandle CostHandle { get; set; }

		internal PswsAuthZUserToken UserToken { get; set; }
	}
}
