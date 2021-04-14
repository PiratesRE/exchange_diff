using System;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal delegate void RemoveItemDelegate<K1, T1>(K1 key, T1 value, RemoveReason reason);
}
