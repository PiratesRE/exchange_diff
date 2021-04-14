using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IQueueVisitor
	{
		void ForEach(Action<IQueueItem> action, bool includeDeferred);
	}
}
