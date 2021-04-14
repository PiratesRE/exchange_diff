using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Services.Core.CssConverter
{
	internal static class CssParser
	{
		public static CssStyleSheet Parse(TextReader reader)
		{
			return CssParser.Parse(reader.ReadToEnd());
		}

		public static CssStyleSheet Parse(string cssString)
		{
			List<CssRule> list = new List<CssRule>();
			cssString = "\n" + cssString;
			cssString = CssParser.HtmlCommentDelimitersRegex.Replace(cssString, string.Empty);
			cssString = CssParser.CssCommentsRegex.Replace(cssString, string.Empty);
			IEnumerable<CssFragment> enumerable = CssParser.SplitCssByMedia(cssString);
			foreach (CssFragment cssFragment in enumerable)
			{
				if (cssFragment.MediaDevices.Count == 0 || cssFragment.MediaDevices.Contains("screen"))
				{
					list.AddRange(CssParser.ParseDeclarations(cssFragment.CssText));
				}
			}
			return new CssStyleSheet(list);
		}

		private static IEnumerable<CssFragment> SplitCssByMedia(string cssString)
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			List<CssFragment> list = new List<CssFragment>();
			string text = "@";
			int num3;
			string text2;
			for (int i = 0; i < cssString.Length; i++)
			{
				if (flag)
				{
					if (cssString[i] != '{' && num == 0)
					{
						text += cssString[i];
					}
					else if (cssString[i] == '{')
					{
						if (num == 0)
						{
							num2 = i + 1;
						}
						num++;
					}
					else if (cssString[i] == '}')
					{
						num--;
						if (num == 0)
						{
							num3 = i;
							flag = false;
							string[] array = text.ToLowerInvariant().Split(CssParser.SpaceDelimiter, 2);
							IList<string> mediaDevices = CssParser.SplitToList(array[1], CssParser.CommaSeparator, int.MaxValue);
							int length = num3 - num2;
							text2 = cssString.Substring(num2, length).Trim();
							if (!string.IsNullOrEmpty(text2))
							{
								list.Add(new CssFragment(mediaDevices, text2));
							}
							num2 = i + 1;
						}
					}
				}
				else if (cssString.Length >= i + 6 && cssString.Substring(i, 6).ToLowerInvariant() == "@media")
				{
					num3 = i;
					text2 = cssString.Substring(num2, num3 - num2).Trim();
					if (!string.IsNullOrEmpty(text2))
					{
						list.Add(new CssFragment(new List<string>(), text2));
					}
					flag = true;
					num = 0;
					text = "@";
				}
			}
			num3 = cssString.Length;
			text2 = cssString.Substring(num2, num3 - num2).Trim();
			if (!string.IsNullOrEmpty(text2))
			{
				list.Add(new CssFragment(new List<string>(), text2));
			}
			return list;
		}

		public static IList<CssRule> ParseDeclarations(string cssString)
		{
			MatchCollection matchCollection = CssParser.CssDefinitionRegex.Matches(cssString);
			IList<CssRule> list = new List<CssRule>();
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				string strValue = match.Groups[1].ToString();
				string propertiesStr = match.Groups[2].ToString();
				IList<string> selectors = CssParser.SplitToList(strValue, CssParser.CommaSeparator, int.MaxValue);
				IList<CssProperty> list2 = CssParser.ParseProperties(propertiesStr);
				if (list2.Count > 0)
				{
					CssRule item = new CssRule(selectors, list2);
					list.Add(item);
				}
			}
			return list;
		}

		public static IList<string> SplitToList(string strValue, char[] separators, int count = 2147483647)
		{
			string[] array = strValue.Split(separators, count, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length <= 0)
			{
				return new List<string>();
			}
			return array.ToList<string>().ConvertAll<string>((string str) => str.Trim());
		}

		public static IList<CssProperty> ParseProperties(string propertiesStr)
		{
			IList<string> list = CssParser.SplitToList(propertiesStr, CssParser.PropertyDelimiter, int.MaxValue);
			IList<CssProperty> list2 = new List<CssProperty>();
			foreach (string strValue in list)
			{
				IList<string> list3 = CssParser.SplitToList(strValue, CssParser.NameValueDelimiter, 2);
				if (list3.Count >= 2)
				{
					CssProperty item = new CssProperty
					{
						Name = list3[0],
						Value = list3[1]
					};
					list2.Add(item);
				}
			}
			return list2;
		}

		private static readonly Regex HtmlCommentDelimitersRegex = new Regex("<!--|-->", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

		private static readonly Regex CssCommentsRegex = new Regex("/\\*(.+?)\\*/", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

		private static readonly Regex CssDefinitionRegex = new Regex("(.+?) *\\{(.*?)\\}", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

		internal static readonly char[] CommaSeparator = new char[]
		{
			','
		};

		internal static readonly char[] PropertyDelimiter = new char[]
		{
			';'
		};

		internal static readonly char[] NameValueDelimiter = new char[]
		{
			':'
		};

		internal static readonly char[] SpaceDelimiter = new char[]
		{
			' '
		};

		internal static readonly char[] ClassDelimiter = new char[]
		{
			'.'
		};
	}
}
