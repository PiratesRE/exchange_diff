using System;

namespace Microsoft.Exchange.Transport
{
	internal interface ICostIndex
	{
		void Add(WaitCondition waitCondition);

		WaitCondition[] TryRemove(bool allowAboveThreshold, object state);
	}
}
