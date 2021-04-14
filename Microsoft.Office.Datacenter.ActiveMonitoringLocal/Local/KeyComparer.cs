using System;
using System.Collections.Generic;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Local
{
	internal class KeyComparer<TKey> : EqualityComparer<TKey>
	{
		public override bool Equals(TKey key1, TKey key2)
		{
			if (typeof(TKey) == typeof(string))
			{
				return string.Equals(key1 as string, key2 as string, StringComparison.OrdinalIgnoreCase);
			}
			return EqualityComparer<TKey>.Default.Equals(key1, key2);
		}

		public override int GetHashCode(TKey key)
		{
			if (typeof(TKey) == typeof(string))
			{
				return (key as string).ToLower().GetHashCode();
			}
			return key.GetHashCode();
		}
	}
}
