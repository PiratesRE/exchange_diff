using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class KillBitList
	{
		public static KillBitList Singleton
		{
			get
			{
				return KillBitList.killBitList;
			}
		}

		public void Clear()
		{
			lock (this.lockObject)
			{
				this.list.Clear();
			}
		}

		public void Add(KilledExtensionEntry entry)
		{
			lock (this.lockObject)
			{
				this.list.Add(entry);
			}
		}

		public void Remove(string extensionId)
		{
			lock (this.lockObject)
			{
				List<KilledExtensionEntry> list = new List<KilledExtensionEntry>();
				foreach (KilledExtensionEntry killedExtensionEntry in this.list)
				{
					if (string.Equals(killedExtensionEntry.ExtensionId, ExtensionDataHelper.FormatExtensionId(extensionId), StringComparison.OrdinalIgnoreCase))
					{
						list.Add(killedExtensionEntry);
					}
				}
				foreach (KilledExtensionEntry item in list)
				{
					this.list.Remove(item);
				}
			}
		}

		public KilledExtensionEntry[] GetList()
		{
			KilledExtensionEntry[] result;
			lock (this.lockObject)
			{
				result = this.list.ToArray();
			}
			return result;
		}

		public bool IsExtensionKilled(string extensionId)
		{
			lock (this.lockObject)
			{
				foreach (KilledExtensionEntry killedExtensionEntry in this.list)
				{
					if (string.Equals(killedExtensionEntry.ExtensionId, ExtensionDataHelper.FormatExtensionId(extensionId), StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		public int Count
		{
			get
			{
				int count;
				lock (this.lockObject)
				{
					count = this.list.Count;
				}
				return count;
			}
		}

		private static KillBitList killBitList = new KillBitList();

		private List<KilledExtensionEntry> list = new List<KilledExtensionEntry>();

		private object lockObject = new object();
	}
}
