using System;
using System.Text;

namespace Microsoft.Exchange.Transport
{
	internal static class TransportProperty
	{
		public static int GetDword(byte[] value)
		{
			if (value.Length != 4)
			{
				throw new TransportPropertyException("invalid length");
			}
			return BitConverter.ToInt32(value, 0);
		}

		public static byte[] PutDword(int value)
		{
			return BitConverter.GetBytes(value);
		}

		public static bool GetBool(byte[] value)
		{
			return TransportProperty.GetDword(value) != 0;
		}

		public static byte[] PutBool(bool value)
		{
			return TransportProperty.PutDword(value ? 1 : 0);
		}

		public static string GetASCIIString(byte[] value)
		{
			int num = Array.IndexOf<byte>(value, 0);
			if (num == -1)
			{
				throw new TransportPropertyException("missing null terminator");
			}
			string @string;
			try
			{
				@string = TransportProperty.CheckedASCII.GetString(value, 0, num);
			}
			catch (DecoderFallbackException innerException)
			{
				throw new TransportPropertyException("invalid encoding", innerException);
			}
			return @string;
		}

		public static byte[] PutASCIIString(string value)
		{
			byte[] array = new byte[TransportProperty.CheckedASCII.GetByteCount(value) + 1];
			TransportProperty.CheckedASCII.GetBytes(value, 0, value.Length, array, 0);
			array[array.Length - 1] = 0;
			return array;
		}

		public static string GetUTF8String(byte[] value)
		{
			int num = Array.IndexOf<byte>(value, 0);
			if (num == -1)
			{
				throw new TransportPropertyException("missing null terminator");
			}
			string @string;
			try
			{
				@string = TransportProperty.CheckedUTF8.GetString(value, 0, num);
			}
			catch (DecoderFallbackException innerException)
			{
				throw new TransportPropertyException("invalid encoding", innerException);
			}
			return @string;
		}

		public static byte[] PutUTF8String(string value)
		{
			byte[] array = new byte[TransportProperty.CheckedUTF8.GetByteCount(value) + 1];
			TransportProperty.CheckedUTF8.GetBytes(value, 0, value.Length, array, 0);
			array[array.Length - 1] = 0;
			return array;
		}

		private static readonly Encoding CheckedASCII = Encoding.GetEncoding("us-ascii", new EncoderExceptionFallback(), new DecoderExceptionFallback());

		private static readonly Encoding CheckedUTF8 = new UTF8Encoding(false, true);
	}
}
