using System;

namespace Microsoft.Exchange.Transport.ResourceMonitoring
{
	internal interface IMeterableQueue
	{
		string Name { get; }

		long Length { get; }
	}
}
