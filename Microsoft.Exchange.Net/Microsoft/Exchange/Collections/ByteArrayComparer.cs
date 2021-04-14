using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Collections
{
	internal sealed class ByteArrayComparer : IEqualityComparer<byte[]>
	{
		private ByteArrayComparer()
		{
		}

		public bool Equals(byte[] left, byte[] right)
		{
			if (object.ReferenceEquals(left, right))
			{
				return true;
			}
			if (left == null || right == null)
			{
				return false;
			}
			if (left.Length != right.Length)
			{
				return false;
			}
			for (int i = 0; i < left.Length; i++)
			{
				if (left[i] != right[i])
				{
					return false;
				}
			}
			return true;
		}

		public int GetHashCode(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			int num = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				num = ((num << 3 | num >> 29) ^ (int)bytes[i]);
			}
			return num;
		}

		public static readonly ByteArrayComparer Instance = new ByteArrayComparer();
	}
}
