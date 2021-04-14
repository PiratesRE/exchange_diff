using System;

namespace Microsoft.Exchange.Transport
{
	internal delegate void ItemRelocked(IQueueItem item, string lockReason, out WaitCondition condition);
}
