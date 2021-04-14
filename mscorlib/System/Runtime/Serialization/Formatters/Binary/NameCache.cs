using System;
using System.Collections.Concurrent;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal sealed class NameCache
	{
		internal object GetCachedValue(string name)
		{
			this.name = name;
			object result;
			if (!NameCache.ht.TryGetValue(name, out result))
			{
				return null;
			}
			return result;
		}

		internal void SetCachedValue(object value)
		{
			NameCache.ht[this.name] = value;
		}

		private static ConcurrentDictionary<string, object> ht = new ConcurrentDictionary<string, object>();

		private string name;
	}
}
