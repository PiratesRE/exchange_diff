using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.SharedCache.Client;

namespace Microsoft.Exchange.HttpProxy
{
	public class MailboxServerCacheEntry : ISharedCacheEntry
	{
		public MailboxServerCacheEntry()
		{
		}

		public MailboxServerCacheEntry(string fqdn, int version, string resourceForest, DateTime lastRefreshTime, bool isSourceCachedData) : this(new BackEndServer(fqdn, version), resourceForest, lastRefreshTime, isSourceCachedData)
		{
		}

		public MailboxServerCacheEntry(BackEndServer backEndServer, string resourceForest, DateTime lastRefreshTime, bool isSourceCachedData)
		{
			this.BackEndServer = backEndServer;
			this.ResourceForest = resourceForest;
			this.LastRefreshTime = lastRefreshTime;
			this.IsSourceCachedData = isSourceCachedData;
		}

		public BackEndServer BackEndServer { get; set; }

		public string ResourceForest { get; set; }

		public DateTime LastRefreshTime { get; set; }

		public bool IsSourceCachedData { get; set; }

		public bool IsDueForRefresh(TimeSpan refreshTimeSpan)
		{
			return DateTime.UtcNow - this.LastRefreshTime > refreshTimeSpan;
		}

		public bool ShouldRefresh(TimeSpan refreshTimeSpan, bool isSourceCachedData)
		{
			return !isSourceCachedData || this.IsSourceCachedData || this.IsDueForRefresh(refreshTimeSpan);
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"CacheEntry(BackEndServer ",
				this.BackEndServer.ToString(),
				"|ResourceForest ",
				this.ResourceForest,
				"|LastRefreshTime ",
				this.LastRefreshTime.ToString("o"),
				"|IsSourceCachedData ",
				this.IsSourceCachedData.ToString(),
				")"
			});
		}

		public void FromByteArray(byte[] bytes)
		{
			if (bytes == null || bytes.Length < 14)
			{
				throw new ArgumentException(string.Format("It's not a valid byte array for MailboxServerCacheEntry, which has at least {0} bytes", 14));
			}
			byte b = bytes[0];
			int num = 1;
			this.LastRefreshTime = DateTime.FromBinary(BitConverter.ToInt64(bytes, num));
			num += 8;
			this.IsSourceCachedData = BitConverter.ToBoolean(bytes, num);
			num++;
			int num2 = BitConverter.ToInt32(bytes, num);
			if (num2 > 0)
			{
				this.BackEndServer = new BackEndServer(Encoding.ASCII.GetString(bytes, num + 8, num2 - 4), BitConverter.ToInt32(bytes, num + 4));
			}
			num += num2 + 4;
			if (num < bytes.Length)
			{
				this.ResourceForest = Encoding.ASCII.GetString(bytes, num, bytes.Length - num);
			}
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[14];
			array[0] = 1;
			int num = 1;
			Buffer.BlockCopy(BitConverter.GetBytes(this.LastRefreshTime.ToBinary()), 0, array, num, 8);
			num += 8;
			array[num] = BitConverter.GetBytes(this.IsSourceCachedData)[0];
			num++;
			IEnumerable<byte> enumerable = null;
			int value = 0;
			if (this.BackEndServer != null)
			{
				enumerable = BitConverter.GetBytes(this.BackEndServer.Version);
				enumerable = enumerable.Concat(Encoding.ASCII.GetBytes(this.BackEndServer.Fqdn));
				value = enumerable.Count<byte>();
			}
			Buffer.BlockCopy(BitConverter.GetBytes(value), 0, array, num, 4);
			IEnumerable<byte> enumerable2 = (enumerable != null) ? array.Concat(enumerable) : array;
			if (!string.IsNullOrEmpty(this.ResourceForest))
			{
				enumerable2 = enumerable2.Concat(Encoding.ASCII.GetBytes(this.ResourceForest));
			}
			return enumerable2.ToArray<byte>();
		}

		public override int GetHashCode()
		{
			int num = this.LastRefreshTime.GetHashCode() ^ this.IsSourceCachedData.GetHashCode();
			if (!string.IsNullOrEmpty(this.ResourceForest))
			{
				num ^= this.ResourceForest.GetHashCode();
			}
			if (this.BackEndServer != null)
			{
				num ^= this.BackEndServer.Version;
				if (!string.IsNullOrEmpty(this.BackEndServer.Fqdn))
				{
					num ^= this.BackEndServer.Fqdn.GetHashCode();
				}
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			MailboxServerCacheEntry mailboxServerCacheEntry = obj as MailboxServerCacheEntry;
			if (mailboxServerCacheEntry != null && this.LastRefreshTime == mailboxServerCacheEntry.LastRefreshTime && this.IsSourceCachedData == mailboxServerCacheEntry.IsSourceCachedData && ((string.IsNullOrEmpty(this.ResourceForest) && string.IsNullOrEmpty(mailboxServerCacheEntry.ResourceForest)) || string.Equals(this.ResourceForest, mailboxServerCacheEntry.ResourceForest)))
			{
				if (this.BackEndServer == null)
				{
					return mailboxServerCacheEntry.BackEndServer == null;
				}
				if (mailboxServerCacheEntry.BackEndServer != null)
				{
					return this.BackEndServer.Version == mailboxServerCacheEntry.BackEndServer.Version && ((string.IsNullOrEmpty(this.BackEndServer.Fqdn) && string.IsNullOrEmpty(mailboxServerCacheEntry.BackEndServer.Fqdn)) || string.Equals(this.BackEndServer.Fqdn, mailboxServerCacheEntry.BackEndServer.Fqdn));
				}
			}
			return false;
		}

		private const int MinimumLength = 14;

		private const byte CurrentSerializationVersion = 1;
	}
}
