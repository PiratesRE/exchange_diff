using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal class LegacyJournalInfoReader
	{
		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public LegacyJournalInfoReader(byte[] data, int start, int length)
		{
			this.data = data;
			this.currentIndex = start;
			this.endIndex = start + length;
		}

		private bool ReadNextValue()
		{
			if (this.currentIndex >= this.endIndex)
			{
				return false;
			}
			this.value = null;
			this.value = this.ReadUTF8String();
			return true;
		}

		private int ReadInt32()
		{
			if (this.currentIndex > this.endIndex - 4)
			{
				throw new TransportPropertyException("TruncatedData");
			}
			int result = BitConverter.ToInt32(this.data, this.currentIndex);
			this.currentIndex += 4;
			return result;
		}

		private string ReadUTF8String()
		{
			int num = this.ReadInt32();
			string result = null;
			if (num < 1 || num > this.endIndex - this.currentIndex)
			{
				throw new TransportPropertyException("invalid string length prefix");
			}
			int num2 = Array.IndexOf<byte>(this.data, 0, this.currentIndex, num);
			if (num2 == -1 || num2 != this.currentIndex + num - 1)
			{
				throw new TransportPropertyException("string is not null-terminated");
			}
			try
			{
				result = LegacyJournalInfoReader.CheckedUTF8.GetString(this.data, this.currentIndex, num - 1);
			}
			catch (DecoderFallbackException innerException)
			{
				throw new TransportPropertyException("invalid encoding", innerException);
			}
			this.currentIndex += num;
			return result;
		}

		public List<string> GetStringProperties()
		{
			List<string> list = new List<string>();
			while (this.ReadNextValue())
			{
				list.Add(this.Value);
			}
			return list;
		}

		public List<ProxyAddress> GetProxyAddressProperties()
		{
			List<ProxyAddress> list = new List<ProxyAddress>();
			while (this.ReadNextValue())
			{
				ProxyAddress proxyAddress = ProxyAddress.Parse(this.Value);
				if (proxyAddress is InvalidProxyAddress)
				{
					throw new TransportPropertyException("invalid proxy address");
				}
				list.Add(proxyAddress);
				this.ReadNextValue();
			}
			return list;
		}

		private static readonly Encoding CheckedUTF8 = new UTF8Encoding(false, true);

		private byte[] data;

		private int currentIndex;

		private int endIndex;

		private string value;
	}
}
