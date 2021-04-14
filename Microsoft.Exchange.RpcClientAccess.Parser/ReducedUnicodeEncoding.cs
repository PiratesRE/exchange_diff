using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal class ReducedUnicodeEncoding : Encoding
	{
		public static bool IsStringConvertible(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] > 'ÿ')
				{
					return false;
				}
			}
			return true;
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (chars.Length - index < count)
			{
				throw new ArgumentOutOfRangeException();
			}
			return count;
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charIndex < 0)
			{
				throw new ArgumentOutOfRangeException("charIndex", "charIndex cannot be less than zero");
			}
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", "charCount cannot be less than zero");
			}
			if (byteIndex < 0)
			{
				throw new ArgumentOutOfRangeException("byteIndex", "byteIndex cannot be less than zero");
			}
			if (charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", "charIndex is larger than length of chars");
			}
			if (chars.Length - charIndex < charCount)
			{
				throw new ArgumentOutOfRangeException("charCount", "charIndex + charCount is larger than length of chars");
			}
			if (byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", "byteIndex is larger than length of bytes");
			}
			int i;
			for (i = charIndex; i < charIndex + charCount; i++)
			{
				bytes[byteIndex++] = (byte)(chars[i] & 'ÿ');
			}
			return i;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "index cannot be less than zero");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "count cannot be less than zero");
			}
			if (bytes.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count", "index + count is larger than length of bytes");
			}
			return count;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (byteIndex < 0)
			{
				throw new ArgumentOutOfRangeException("byteIndex", "byteIndex cannot be less than zero");
			}
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", "byteCount cannot be less than zero");
			}
			if (charIndex < 0)
			{
				throw new ArgumentOutOfRangeException("charIndex", "charIndex cannot be less than zero");
			}
			if (byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", "byteIndex is larger than length of chars");
			}
			if (bytes.Length - byteIndex < byteCount)
			{
				throw new ArgumentOutOfRangeException("byteCount", "byteIndex + byteCount is larger than length of bytes");
			}
			if (charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", "charIndex is larger than length of chars");
			}
			for (int i = 0; i < byteCount; i++)
			{
				chars[charIndex + i] = (char)bytes[byteIndex + i];
			}
			return byteCount;
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", "charCount cannot be less than zero");
			}
			return charCount;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", "byteCount cannot be less than zero");
			}
			return byteCount;
		}
	}
}
