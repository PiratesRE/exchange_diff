using System;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.SharedCache.Client;

namespace Microsoft.Exchange.HttpProxy
{
	public class AnchorMailboxCacheEntry : ISharedCacheEntry
	{
		public ADObjectId Database { get; set; }

		public string DomainName { get; set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.Database,
				'~',
				this.DomainName,
				'~',
				(this.Database == null) ? string.Empty : this.Database.PartitionFQDN
			});
		}

		public void FromByteArray(byte[] bytes)
		{
			if (bytes != null)
			{
				if (bytes.Length == 0)
				{
					throw new ArgumentException("Empty byte array for AnchorMailboxCacheEntry!");
				}
				int num = (bytes[0] == 1) ? 17 : 1;
				byte[] array = null;
				if (bytes[0] == 1)
				{
					if (bytes.Length < 17)
					{
						throw new ArgumentException(string.Format("There should be at least {0} bytes for the database GUID", 16));
					}
					array = new byte[16];
					Array.Copy(bytes, 1, array, 0, 16);
				}
				string domainName = null;
				string partitionFQDN = null;
				if (bytes.Length > num)
				{
					string @string = Encoding.ASCII.GetString(bytes, num, bytes.Length - num);
					Utilities.GetTwoSubstrings(@string, '~', out domainName, out partitionFQDN);
					this.DomainName = domainName;
				}
				if (array != null)
				{
					this.Database = new ADObjectId(new Guid(array), partitionFQDN);
				}
			}
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[]
			{
				(this.Database == null) ? 0 : 1
			};
			if (this.Database != null)
			{
				Guid objectGuid = this.Database.ObjectGuid;
				array = array.Concat(this.Database.ObjectGuid.ToByteArray()).ToArray<byte>();
			}
			if (!string.IsNullOrEmpty(this.DomainName))
			{
				array = array.Concat(Encoding.ASCII.GetBytes(this.DomainName)).ToArray<byte>();
			}
			if (this.Database != null && !string.IsNullOrEmpty(this.Database.PartitionFQDN))
			{
				array = array.Concat(Encoding.ASCII.GetBytes(new char[]
				{
					'~'
				})).ToArray<byte>();
				array = array.Concat(Encoding.ASCII.GetBytes(this.Database.PartitionFQDN)).ToArray<byte>();
			}
			return array;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (!string.IsNullOrEmpty(this.DomainName))
			{
				num ^= this.DomainName.GetHashCode();
			}
			if (this.Database != null)
			{
				num ^= this.Database.ObjectGuid.GetHashCode();
				if (!string.IsNullOrEmpty(this.Database.PartitionFQDN))
				{
					num ^= this.Database.PartitionFQDN.GetHashCode();
				}
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			AnchorMailboxCacheEntry anchorMailboxCacheEntry = obj as AnchorMailboxCacheEntry;
			if (anchorMailboxCacheEntry != null && ((string.IsNullOrEmpty(this.DomainName) && string.IsNullOrEmpty(anchorMailboxCacheEntry.DomainName)) || string.Equals(this.DomainName, anchorMailboxCacheEntry.DomainName)))
			{
				if (this.Database == null)
				{
					return anchorMailboxCacheEntry.Database == null;
				}
				if (anchorMailboxCacheEntry.Database != null && this.Database.ObjectGuid == anchorMailboxCacheEntry.Database.ObjectGuid)
				{
					return (string.IsNullOrEmpty(this.Database.PartitionFQDN) && string.IsNullOrEmpty(anchorMailboxCacheEntry.Database.PartitionFQDN)) || string.Equals(this.Database.PartitionFQDN, anchorMailboxCacheEntry.Database.PartitionFQDN);
				}
			}
			return false;
		}

		private const int GuidLengthInBytes = 16;

		private const char Separator = '~';
	}
}
