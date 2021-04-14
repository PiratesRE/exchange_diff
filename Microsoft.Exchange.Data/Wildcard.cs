using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[ImmutableObject(true)]
	[Serializable]
	internal static class Wildcard
	{
		public static string ConvertToRegexPattern(string wildcardString)
		{
			if (string.IsNullOrEmpty(wildcardString))
			{
				throw new ArgumentNullException("wildcardString");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("^");
			stringBuilder.Append(Regex.Escape(wildcardString));
			stringBuilder.Replace(Wildcard.escapedAnyCharacterWildcardString, ".*");
			stringBuilder.Replace(Wildcard.escapedOneCharacterWildcardString, ".");
			stringBuilder.Append("$");
			return stringBuilder.ToString();
		}

		public const char AnyCharacterWildcard = '*';

		public const string AnyCharacterWildcardString = "*";

		public const char OneCharacterWildcard = '?';

		public const string OneCharacterWildcardString = "?";

		private static readonly string escapedAnyCharacterWildcardString = Regex.Escape("*");

		private static readonly string escapedOneCharacterWildcardString = Regex.Escape("?");
	}
}
