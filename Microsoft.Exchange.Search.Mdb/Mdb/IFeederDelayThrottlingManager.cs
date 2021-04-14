using System;

namespace Microsoft.Exchange.Search.Mdb
{
	internal interface IFeederDelayThrottlingManager
	{
		TimeSpan DelayForThrottling();
	}
}
