using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.AirSync
{
	public class EasMailboxSessionCacheResult
	{
		public EasMailboxSessionCacheResult()
		{
		}

		internal EasMailboxSessionCacheResult(int count)
		{
			this.Entries = null;
			this.Count = count;
			this.Initialize();
		}

		internal EasMailboxSessionCacheResult(List<MruCacheDiagnosticEntryInfo> entries)
		{
			if (entries != null)
			{
				this.Entries = new EasMailboxSessionCacheResultItem[entries.Count];
				for (int i = 0; i < entries.Count; i++)
				{
					this.Entries[i] = new EasMailboxSessionCacheResultItem(entries[i].Identifier, entries[i].TimeToLive);
				}
				this.Count = entries.Count;
			}
			else
			{
				this.Entries = null;
				this.Count = 0;
			}
			this.Initialize();
		}

		private void Initialize()
		{
			this.MaxCacheSize = GlobalSettings.MailboxSessionCacheMaxSize;
			this.CacheTimeout = GlobalSettings.MailboxSessionCacheTimeout.ToString();
			this.CacheEfficiency = MailboxSessionCache.GetCacheEfficiency().ToString("P2");
			this.DiscardedSessions = MailboxSessionCache.DiscardedSessions;
		}

		public int DiscardedSessions { get; set; }

		public int MaxCacheSize { get; set; }

		public string CacheTimeout { get; set; }

		public string CacheEfficiency { get; set; }

		public int Count { get; set; }

		[XmlArray("CacheEntries")]
		[XmlArrayItem("CacheEntry")]
		public EasMailboxSessionCacheResultItem[] Entries { get; set; }
	}
}
