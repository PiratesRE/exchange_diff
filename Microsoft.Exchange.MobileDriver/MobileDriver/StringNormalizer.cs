using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal static class StringNormalizer
	{
		public static string NormalizeEndOfLines(string original)
		{
			foreach (string oldValue in StringNormalizer.EndOfLinesOtherThanLf)
			{
				original = original.Replace(oldValue, '\n'.ToString());
			}
			return original;
		}

		public static string TrimTrailingEndOfLines(string original)
		{
			return original.TrimEnd(new char[]
			{
				'\r',
				'\n',
				'\u0085',
				'\u2028'
			});
		}

		public static string TrimTrailingAndNormalizeEol(string original)
		{
			return StringNormalizer.NormalizeEndOfLines(StringNormalizer.TrimTrailingEndOfLines(original));
		}

		private const char Cr = '\r';

		private const char Lf = '\n';

		private const char Nel = '\u0085';

		private const char LSep = '\u2028';

		private const string CrStr = "\r";

		private const string LfStr = "\n";

		private const string NelStr = "\u0085";

		private const string Crlf = "\r\n";

		private const string CrNel = "\r\u0085";

		private static readonly string[] EndOfLinesOtherThanLf = new string[]
		{
			"\r\n",
			"\r\u0085",
			'\u0085'.ToString(),
			'\u2028'.ToString(),
			'\r'.ToString()
		};
	}
}
