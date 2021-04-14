using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	internal class CompressedAddressComparer : IEqualityComparer<byte[]>
	{
		bool IEqualityComparer<byte[]>.Equals(byte[] aBytes, byte[] bBytes)
		{
			if (aBytes.Length != bBytes.Length)
			{
				return false;
			}
			for (int i = 0; i < aBytes.Length; i++)
			{
				if (aBytes[i] != bBytes[i])
				{
					return false;
				}
			}
			return true;
		}

		public int GetHashCode(byte[] bytes)
		{
			string @string = Encoding.ASCII.GetString(bytes);
			return @string.GetHashCode();
		}
	}
}
