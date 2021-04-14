using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal interface ICacheDirectorySession
	{
		void Insert(IConfigurable objectToSave, IEnumerable<PropertyDefinition> properties, List<Tuple<string, KeyType>> keys = null, int secondsTimeout = 2147483646, CacheItemPriority priority = CacheItemPriority.Default);
	}
}
