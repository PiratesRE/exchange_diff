using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.SharedCache.Client;

namespace Microsoft.Exchange.HttpProxy
{
	public class CafeArraysCacheEntry : ISharedCacheEntry
	{
		public CafeArraysCacheEntry()
		{
		}

		public CafeArraysCacheEntry(string cafeArrayPreferenceList)
		{
			if (string.IsNullOrEmpty(cafeArrayPreferenceList))
			{
				throw new ArgumentNullException("cafeArrayPreferenceList", "Argument in Null or Empty");
			}
			this.cafeArrayPreferenceList = cafeArrayPreferenceList;
		}

		public string CafeArrayPreferenceList
		{
			get
			{
				return this.cafeArrayPreferenceList;
			}
		}

		public override string ToString()
		{
			return this.CafeArrayPreferenceList;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			CafeArraysCacheEntry cafeArraysCacheEntry = obj as CafeArraysCacheEntry;
			string b = null;
			if (cafeArraysCacheEntry != null)
			{
				b = cafeArraysCacheEntry.CafeArrayPreferenceList;
			}
			return string.Equals(this.CafeArrayPreferenceList, b, StringComparison.OrdinalIgnoreCase);
		}

		public string GetPreferredRoutingUnit()
		{
			string[] array = this.CafeArrayPreferenceList.Split(new char[]
			{
				','
			});
			if (array.Length > 4 || array.Length < 1)
			{
				throw new ArgumentOutOfRangeException("CafeArrayPreferenceList", "Length was {0}" + array.Length);
			}
			return array[0];
		}

		public void FromByteArray(byte[] bytes)
		{
			if (bytes == null || bytes.Length < 5)
			{
				throw new ArgumentException(string.Format("It's not a valid byte array for CafeArrayCacheEntry, which has at least {0} bytes", 5));
			}
			byte b = bytes[0];
			int num = 1;
			int num2 = BitConverter.ToInt32(bytes, num);
			num += 4;
			if (num2 > 0)
			{
				this.cafeArrayPreferenceList = Encoding.ASCII.GetString(bytes, num, num2);
				return;
			}
			this.cafeArrayPreferenceList = CafeArraysCacheEntry.DefaultCafeArrayPreferenceList;
		}

		public byte[] ToByteArray()
		{
			IEnumerable<byte> enumerable = CafeArraysCacheEntry.Version;
			int value = 0;
			bool flag = false;
			if (!string.IsNullOrEmpty(this.CafeArrayPreferenceList))
			{
				flag = true;
				value = this.CafeArrayPreferenceList.Length;
			}
			enumerable = enumerable.Concat(BitConverter.GetBytes(value));
			if (flag)
			{
				enumerable = enumerable.Concat(Encoding.ASCII.GetBytes(this.CafeArrayPreferenceList));
			}
			return enumerable.ToArray<byte>();
		}

		internal const int MaxPreferenceListElements = 4;

		internal const char Delimiter = ',';

		private const int MinimumLength = 5;

		private const byte CurrentSerializationVersion = 1;

		private static readonly byte[] Version = new byte[]
		{
			1
		};

		private static readonly string DefaultCafeArrayPreferenceList = string.Format("{0}{1}{0}{1}{0}{1}{0}", -1, ',');

		private string cafeArrayPreferenceList;
	}
}
