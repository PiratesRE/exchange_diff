using System;
using System.Collections.Concurrent;

namespace Microsoft.Exchange.Data
{
	internal class PiiMap
	{
		public PiiMap()
		{
			for (int i = 0; i < this.maps.Length; i++)
			{
				this.maps[i] = new ConcurrentDictionary<string, string>(2, 10000, StringComparer.OrdinalIgnoreCase);
			}
		}

		public string this[string key]
		{
			get
			{
				string result = null;
				foreach (ConcurrentDictionary<string, string> concurrentDictionary in this.maps)
				{
					if (concurrentDictionary.TryGetValue(key, out result))
					{
						break;
					}
				}
				return result;
			}
			set
			{
				this.CleanUpOldDataIfNeeded();
				if (!string.IsNullOrEmpty(key))
				{
					this.maps[this.currentMap][key] = value;
				}
			}
		}

		private void CleanUpOldDataIfNeeded()
		{
			int num = (this.currentMap == 0) ? 1 : 0;
			if (this.maps[this.currentMap].Count >= 10000)
			{
				if (this.maps[num].Count >= 10000)
				{
					this.maps[num].Clear();
				}
				this.currentMap = num;
			}
		}

		private const int Capacity = 10000;

		private ConcurrentDictionary<string, string>[] maps = new ConcurrentDictionary<string, string>[2];

		private int currentMap;
	}
}
