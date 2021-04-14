using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	internal sealed class AddressHashes
	{
		public AddressHashes(byte[] hashArray)
		{
			if (hashArray == null)
			{
				throw new ArgumentNullException("hashArray");
			}
			this.List = new SortedList<uint, byte>(hashArray.Length / 4);
			for (int i = 0; i < hashArray.Length / 4; i++)
			{
				int num = i * 4;
				uint num2 = (uint)hashArray[num + 3];
				num2 <<= 8;
				num2 |= (uint)hashArray[num + 2];
				num2 <<= 8;
				num2 |= (uint)hashArray[num + 1];
				num2 <<= 8;
				num2 |= (uint)hashArray[num];
				this.List.Add(num2, 0);
			}
		}

		public AddressHashes()
		{
			this.List = new SortedList<uint, byte>();
		}

		public void Add(string addressOrDomain)
		{
			if (string.IsNullOrEmpty(addressOrDomain))
			{
				return;
			}
			if (addressOrDomain[0] == '@')
			{
				addressOrDomain = addressOrDomain.Substring(1);
				if (!SmtpAddress.IsValidDomain(addressOrDomain))
				{
					return;
				}
			}
			else if (!RoutingAddress.IsValidAddress(addressOrDomain))
			{
				return;
			}
			uint key = (uint)this.hasher.GetHash(addressOrDomain);
			if (this.List.ContainsKey(key))
			{
				return;
			}
			this.List.Add(key, 0);
		}

		public byte[] GetBytes()
		{
			byte[] array = new byte[this.List.Count * 4];
			for (int i = 0; i < this.List.Count; i++)
			{
				int num = i * 4;
				uint num2 = this.List.Keys[i];
				array[num] = (byte)(num2 & 255U);
				array[num + 1] = (byte)((num2 & 65280U) >> 8);
				array[num + 2] = (byte)((num2 & 16711680U) >> 16);
				array[num + 3] = (byte)((num2 & 4278190080U) >> 24);
			}
			return array;
		}

		public uint[] GetHashArray()
		{
			uint[] array = new uint[this.List.Count];
			this.List.Keys.CopyTo(array, 0);
			return array;
		}

		public bool Contains(RoutingAddress address)
		{
			if (!AddressHashes.IsValidAddress(address))
			{
				return false;
			}
			uint key = (uint)this.hasher.GetHash((string)address);
			if (this.List.ContainsKey(key))
			{
				return true;
			}
			string domainPart = address.DomainPart;
			key = (uint)this.hasher.GetHash(domainPart);
			return this.List.ContainsKey(key);
		}

		public int Count
		{
			get
			{
				if (this.List != null)
				{
					return this.List.Count;
				}
				return 0;
			}
		}

		private static bool IsValidAddress(RoutingAddress address)
		{
			return address.IsValid && !(address == RoutingAddress.NullReversePath);
		}

		internal SortedList<uint, byte> List;

		private StringHasher hasher = new StringHasher();
	}
}
