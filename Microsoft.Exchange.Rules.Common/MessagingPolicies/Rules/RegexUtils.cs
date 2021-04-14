using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal static class RegexUtils
	{
		public static string ConvertLegacyRegexToTpl(string legacyPattern)
		{
			string text = legacyPattern;
			foreach (string text2 in RegexUtils.escapableItems)
			{
				text = text.Replace(text2, RegexUtils.escapeChar + text2);
			}
			return text;
		}

		public static ShortList<string> ConvertLegacyRegexToTpl(ShortList<string> legacyPatterns)
		{
			ShortList<string> shortList = new ShortList<string>();
			foreach (string legacyPattern in legacyPatterns)
			{
				shortList.Add(RegexUtils.ConvertLegacyRegexToTpl(legacyPattern));
			}
			return shortList;
		}

		private static readonly string[] escapableItems = new string[]
		{
			"[",
			"]",
			".",
			"?",
			"+",
			"{",
			"}"
		};

		private static readonly char escapeChar = '\\';
	}
}
