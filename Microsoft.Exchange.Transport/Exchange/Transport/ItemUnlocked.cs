using System;

namespace Microsoft.Exchange.Transport
{
	internal delegate bool ItemUnlocked(IQueueItem item, AccessToken token);
}
