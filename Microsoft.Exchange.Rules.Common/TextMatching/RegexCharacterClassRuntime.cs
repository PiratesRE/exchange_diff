using System;

namespace Microsoft.Exchange.TextMatching
{
	internal sealed class RegexCharacterClassRuntime
	{
		private RegexCharacterClassRuntime()
		{
		}

		public static bool IsSpace(int ch)
		{
			return char.IsWhiteSpace((char)ch);
		}

		public static bool IsNonSpace(int ch)
		{
			return !RegexCharacterClassRuntime.IsEOF(ch) && !char.IsWhiteSpace((char)ch);
		}

		public static bool IsDigit(int ch)
		{
			return char.IsDigit((char)ch);
		}

		public static bool IsNonDigit(int ch)
		{
			return !RegexCharacterClassRuntime.IsEOF(ch) && !char.IsDigit((char)ch);
		}

		public static bool IsWord(int ch)
		{
			return char.IsLetterOrDigit((char)ch);
		}

		public static bool IsNonWord(int ch)
		{
			return !RegexCharacterClassRuntime.IsWord(ch);
		}

		public static bool IsBegin(int ch)
		{
			return ch == 1;
		}

		public static bool IsEnd(int ch)
		{
			return RegexCharacterClassRuntime.IsEOF(ch);
		}

		private static bool IsEOF(int ch)
		{
			return ch == -1;
		}
	}
}
