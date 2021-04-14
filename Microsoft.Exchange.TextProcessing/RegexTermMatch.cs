using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.TextProcessing
{
	internal class RegexTermMatch : IMatch
	{
		internal RegexTermMatch(IEnumerable<string> terms)
		{
			if (this.IsEmpty(terms))
			{
				throw new ArgumentException(Strings.EmptyTermSet);
			}
			if (terms.Any(new Func<string, bool>(string.IsNullOrEmpty)))
			{
				throw new ArgumentException(Strings.InvalidTerm);
			}
			this.regex = new Regex(this.GetPattern(terms), RegexOptions.ExplicitCapture);
		}

		public bool IsMatch(TextScanContext data)
		{
			return this.regex.IsMatch(data.NormalizedData);
		}

		private bool IsEmpty(IEnumerable<string> collection)
		{
			return collection == null || !collection.GetEnumerator().MoveNext();
		}

		private string GetPattern(IEnumerable<string> terms)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			stringBuilder.Append("(^|\\W)(");
			foreach (string text in terms)
			{
				if (!flag)
				{
					stringBuilder.Append("|");
				}
				foreach (char c in text)
				{
					if (".$^{}[](|)*+?\\".Contains(c))
					{
						stringBuilder.Append('\\');
						stringBuilder.Append(c);
					}
					else
					{
						stringBuilder.Append(char.ToUpperInvariant(c));
					}
				}
				flag = false;
			}
			stringBuilder.Append(")(\\W|$)");
			return stringBuilder.ToString();
		}

		private const string SpecialCharacterString = ".$^{}[](|)*+?\\";

		private const string WordBoundaryRegexFragment = "\\b";

		private const string AlternationFragment = "|";

		private const char EscapeCharacter = '\\';

		private const string PatternPrefix = "(^|\\W)(";

		private const string PatternSuffix = ")(\\W|$)";

		private Regex regex;
	}
}
