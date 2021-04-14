using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal class CacheEntry
	{
		public CacheEntry(List<ObjectTuple> simpleADObjectList)
		{
			this.SimpleADObjectList = simpleADObjectList;
			this.keys = new List<string>[CacheEntry.KeyLength];
		}

		public List<ObjectTuple> SimpleADObjectList { get; internal set; }

		public bool Invalid { get; internal set; }

		public List<string> this[KeyType keyType]
		{
			get
			{
				if (this.keys[this.GetKeyIndex(keyType)] == null)
				{
					this.keys[this.GetKeyIndex(keyType)] = new List<string>(5);
				}
				return this.keys[this.GetKeyIndex(keyType)];
			}
			set
			{
				this.keys[this.GetKeyIndex(keyType)] = value;
			}
		}

		public void ClearKeys()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (this.keys[i] != null)
				{
					List<string> list = null;
					list = Interlocked.Exchange<List<string>>(ref this.keys[i], list);
					if (list != null)
					{
						foreach (string key in list)
						{
							CacheManager.Instance.KeyTable.Remove(key, null);
						}
						list.Clear();
					}
				}
			}
		}

		public void ClearKey(KeyType keyType)
		{
			int keyIndex = this.GetKeyIndex(keyType);
			if (this.keys[keyIndex] != null)
			{
				List<string> list = null;
				list = Interlocked.Exchange<List<string>>(ref this.keys[keyIndex], list);
				if (list != null)
				{
					foreach (string key in list)
					{
						CacheManager.Instance.KeyTable.Remove(key, null);
					}
					list.Clear();
				}
			}
		}

		private int GetKeyIndex(KeyType keyType)
		{
			if (keyType == KeyType.None)
			{
				return 0;
			}
			int num = 1;
			for (int i = 0; i < 32; i++)
			{
				if ((keyType & (KeyType)num) != KeyType.None)
				{
					return i + 1;
				}
				num <<= 1;
			}
			return 0;
		}

		private List<string>[] keys;

		private static int KeyLength = Enum.GetValues(typeof(KeyType)).Length;
	}
}
