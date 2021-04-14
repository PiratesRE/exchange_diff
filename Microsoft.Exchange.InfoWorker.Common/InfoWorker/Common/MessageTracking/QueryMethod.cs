using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal delegate T QueryMethod<K, T>(K key, T currentCache, out KeyValuePair<K, T>[] additionalRecords);
}
