using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public static class HtmlUtility
	{
		public static Dictionary<string, string> GetHiddenFormInputs(string formHtml)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Regex regex = new Regex("<\\s*?input ?", RegexOptions.IgnoreCase);
			MatchCollection matchCollection = regex.Matches(formHtml);
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				Dictionary<string, string> dictionary2 = HtmlUtility.ParseTag(formHtml, match.Index);
				if (dictionary2.ContainsKey("type") && dictionary2["type"].Equals("hidden", StringComparison.InvariantCultureIgnoreCase) && dictionary2.ContainsKey("name"))
				{
					string value = dictionary2.ContainsKey("value") ? dictionary2["value"] : string.Empty;
					dictionary.Add(dictionary2["name"], value);
				}
			}
			return dictionary;
		}

		public static string GetTagInnerHtml(string tagName, string html, string id = null)
		{
			string pattern = string.Format("<\\s*?{0}[^>]*?>(?<InnerText>.+?)</(\\s|{0})\\s*?>", tagName);
			if (!string.IsNullOrEmpty(id))
			{
				pattern = string.Format("<\\s*?{0}[^>]*?id=[\"'\\s]?{1}[\"'\\s].*?>(?<InnerText>.+?)</(\\s|{0})\\s*?>", tagName, id);
			}
			return HtmlUtility.GetNamedRegExCapture(pattern, html, "InnerText", RegexOptions.IgnoreCase | RegexOptions.Singleline);
		}

		public static string GetJavaScriptVariableValue(string variableName, string html)
		{
			string pattern = string.Format("{0}\\s*=\\s*((\"(?<Value>[^\"]*)\"\\s*;)|'(?<Value>[^']*)'\\s*;|\\s*(?<Value>[^;]*)\\s*;)", variableName);
			return HtmlUtility.GetNamedRegExCapture(pattern, html, "Value", RegexOptions.IgnoreCase | RegexOptions.Singleline);
		}

		public static string GetNamedRegExCapture(string pattern, string text, string captureName, RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline)
		{
			Regex regex = new Regex(pattern, options);
			Match match = regex.Match(text);
			if (!match.Success)
			{
				return null;
			}
			return match.Groups[captureName].Value;
		}

		public static Dictionary<string, string> ParseTag(string html, int start = 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			char c = '\0';
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			int num = 0;
			string text = string.Empty;
			int i = start;
			int length = html.Length;
			while (i < length)
			{
				char c2 = html[i];
				if (!flag2)
				{
					if (char.IsWhiteSpace(c2))
					{
						i++;
						continue;
					}
					if (c2 == '=' || c2 == '/' || c2 == '>')
					{
						if (!string.IsNullOrEmpty(text))
						{
							flag = (c2 == '=');
						}
					}
					else if (c2 != '<')
					{
						flag2 = true;
						num = i;
					}
				}
				if (!flag)
				{
					if (flag2 && (char.IsWhiteSpace(c2) || c2 == '=' || c2 == '/' || c2 == '>'))
					{
						text = html.Substring(num, i - num);
						text = text.ToLowerInvariant();
						dictionary[text] = string.Empty;
						flag2 = false;
						flag = (c2 == '=');
					}
				}
				else
				{
					bool flag4 = false;
					if (flag3)
					{
						if (c2 == '"' || c2 == '\'')
						{
							c = c2;
							flag3 = false;
						}
						else if (char.IsWhiteSpace(c2) || c2 == '/' || c2 == '>')
						{
							flag4 = true;
						}
					}
					else
					{
						flag4 = (c2 == c);
					}
					if (i == length - 1)
					{
						flag4 = true;
					}
					if (flag4)
					{
						if (!string.IsNullOrEmpty(text))
						{
							int num2 = (c != '\0') ? 1 : 0;
							int num3 = (i == length - 1) ? 1 : 0;
							dictionary[text] = html.Substring(num + num2, i - num - num2 + num3);
							text = string.Empty;
						}
						flag3 = true;
						c = '\0';
						flag = false;
						flag2 = false;
					}
				}
				if (c2 == '>' && flag3)
				{
					break;
				}
				i++;
			}
			return dictionary;
		}
	}
}
