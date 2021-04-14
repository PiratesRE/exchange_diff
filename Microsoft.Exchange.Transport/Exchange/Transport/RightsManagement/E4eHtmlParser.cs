using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal static class E4eHtmlParser
	{
		public static string GetElement(string stringToSearch, string tagName, string attributeName, string attributeValue)
		{
			string text;
			string text2;
			return E4eHtmlParser.GetElement(stringToSearch, tagName, attributeName, attributeValue, out text, out text2);
		}

		public static string GetElement(string stringToSearch, string tagName, string attributeName, string attributeValue, out string innerHtml, out string parsedAttributeValue)
		{
			if (tagName == null && attributeName == null)
			{
				throw new ArgumentNullException("Either tagName or attributeName must be non-null.");
			}
			if (attributeValue != null && attributeName == null)
			{
				throw new ArgumentNullException("If attributeValue is specified then attributeName must be non-null.");
			}
			if (string.IsNullOrEmpty(stringToSearch))
			{
				throw new ArgumentNullException("stringToSearch cannot be null or empty.");
			}
			innerHtml = null;
			string result = null;
			Match match = E4eHtmlParser.MatchStartTag(stringToSearch, tagName, attributeName, attributeValue, out parsedAttributeValue);
			if (match.Success)
			{
				if (string.IsNullOrEmpty(tagName))
				{
					Match match2 = E4eHtmlParser.tagRegex.Match(stringToSearch, match.Index);
					if (match2.Success && match2.Groups.Count > 0 && match2.Groups[1].Value.Length > 0)
					{
						tagName = match2.Groups[1].Value;
					}
				}
				int num = -1;
				int num2 = -1;
				if (!string.IsNullOrEmpty(tagName))
				{
					num2 = E4eHtmlParser.FindEndTag(stringToSearch, tagName, match.Index, out num);
				}
				if (num2 >= 0)
				{
					int length = num - match.Index + 1;
					result = stringToSearch.Substring(match.Index, length);
					int num3 = match.Index + match.Groups[0].Length;
					int length2 = num2 - num3;
					innerHtml = stringToSearch.Substring(num3, length2);
				}
				else
				{
					result = match.Groups[0].Value;
					innerHtml = string.Empty;
				}
			}
			return result;
		}

		public static Match MatchStartTag(string stringToSearch, string tagName, string attributeName, string attributeValue, out string parsedAttributeValue)
		{
			if (tagName == null && attributeName == null)
			{
				throw new ArgumentNullException("Either tagName or attributeName must be non-null.");
			}
			if (attributeValue != null && attributeName == null)
			{
				throw new ArgumentNullException("If attributeValue is specified then attributeName must be non-null.");
			}
			if (string.IsNullOrEmpty(stringToSearch))
			{
				throw new ArgumentNullException("stringToSearch cannot be null or empty.");
			}
			parsedAttributeValue = null;
			bool flag = false;
			string format;
			if (!string.IsNullOrEmpty(attributeName))
			{
				if (!string.IsNullOrEmpty(attributeValue))
				{
					format = "<{0}\\s+[^>]*\\b{1}\\b\\s*=\\s*(?<quotes>[\"']?){2}\\k<quotes>(>|\\s+[^>]*>|/>)";
				}
				else
				{
					format = "<{0}\\s+[^>]*\\b{1}\\b(?:=\\s*(?<quotes>[\"']?){2}\\k<quotes>(?:>|\\s+[^>]*>|/>)|>|\\s+[^>]*>|/>)";
					flag = true;
				}
			}
			else
			{
				format = "<{0}\\s*(>|\\s+[^>]*\\b{1}\\b\\s*=\\s*(?<quotes>[\"']?){2}\\k<quotes>(>|\\s+[^>]*>|/>|\\s+/>)|/>|\\s+/>)";
			}
			if (string.IsNullOrEmpty(tagName))
			{
				tagName = "(?:[^\\s>]+)";
			}
			else
			{
				tagName = Regex.Escape(tagName);
			}
			if (string.IsNullOrEmpty(attributeName))
			{
				attributeName = "(?:[^\\s>]+)";
			}
			else
			{
				attributeName = Regex.Escape(attributeName);
			}
			if (string.IsNullOrEmpty(attributeValue))
			{
				attributeValue = "([^>\"]*)";
			}
			else
			{
				attributeValue = Regex.Escape(attributeValue);
			}
			string pattern = string.Format(format, tagName, attributeName, attributeValue);
			Match match = Regex.Match(stringToSearch, pattern, RegexOptions.Compiled);
			if (match.Success && flag && match.Groups.Count > 1 && match.Groups[1].Value != null)
			{
				parsedAttributeValue = match.Groups[1].Value;
			}
			return match;
		}

		public static int FindEndTag(string stringToSearch, string tagName, int startIndex, out int endIndex)
		{
			if (string.IsNullOrEmpty(stringToSearch))
			{
				throw new ArgumentNullException("stringToSearch cannot be null or empty.");
			}
			int num = 0;
			int num2 = startIndex;
			int length = stringToSearch.Length;
			string text = "<" + tagName;
			string text2 = "</" + tagName + ">";
			endIndex = -1;
			do
			{
				int num3 = stringToSearch.IndexOf(text, num2);
				int num4 = stringToSearch.IndexOf(text2, num2);
				if (num3 >= 0 && (num4 == -1 || num3 < num4))
				{
					num++;
					num2 = num3 + text.Length;
				}
				else if (num4 >= 0 && (num3 == -1 || num4 < num3))
				{
					num--;
					num2 = num4 + text2.Length;
				}
				else
				{
					num2 = -1;
				}
			}
			while (num > 0 && num2 < length && num2 >= 0);
			if (num2 >= 0)
			{
				num2 -= text2.Length;
				endIndex = stringToSearch.IndexOf('>', num2);
			}
			return num2;
		}

		private static readonly Regex tagRegex = new Regex("<([\\w:]+)[\\s+>]", RegexOptions.Compiled);
	}
}
