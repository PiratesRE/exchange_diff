using System;
using System.Text;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal static class String8Encodings
	{
		public static Encoding ReducedUnicode
		{
			get
			{
				return String8Encodings.reducedUnicode;
			}
		}

		public static Encoding TemporaryDefault
		{
			get
			{
				return CTSGlobals.AsciiEncoding;
			}
		}

		public static bool IsValidString8Encoding(Encoding encoding)
		{
			return encoding.GetByteCount(String8Encodings.emptyString) == 1;
		}

		public static void ThrowIfInvalidString8Encoding(Encoding encoding)
		{
			if (!String8Encodings.IsValidString8Encoding(encoding))
			{
				string message = string.Format("{0} is not supported as a String8 codepage", encoding.WebName);
				throw new ArgumentException(message);
			}
		}

		public static bool TryGetEncoding(int codePage, out Encoding encoding)
		{
			encoding = null;
			bool result;
			try
			{
				encoding = CodePageMap.GetEncoding(codePage);
				result = true;
			}
			catch (ArgumentException)
			{
				result = false;
			}
			catch (NotSupportedException)
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetEncoding(int codePage, Encoding fallbackEncoding, out Encoding encoding)
		{
			encoding = null;
			if (fallbackEncoding == null)
			{
				throw new ArgumentNullException("fallbackEncoding");
			}
			if (codePage == 4095)
			{
				encoding = fallbackEncoding;
				return true;
			}
			return String8Encodings.TryGetEncoding(codePage, out encoding);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static String8Encodings()
		{
			char[] array = new char[1];
			String8Encodings.emptyString = array;
			String8Encodings.reducedUnicode = new ReducedUnicodeEncoding();
		}

		public const int CodePageUnspecified = 4095;

		private static readonly char[] emptyString;

		private static ReducedUnicodeEncoding reducedUnicode;
	}
}
