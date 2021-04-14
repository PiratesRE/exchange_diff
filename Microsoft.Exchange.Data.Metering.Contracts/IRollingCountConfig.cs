using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal interface IRollingCountConfig : ICountedConfig
	{
		TimeSpan WindowInterval { get; }

		TimeSpan WindowBucketSize { get; }
	}
}
