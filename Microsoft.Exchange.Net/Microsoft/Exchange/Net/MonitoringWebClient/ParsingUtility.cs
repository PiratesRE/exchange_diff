using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal static class ParsingUtility
	{
		public static string ParseJsonField(HttpWebResponseWrapper response, string name)
		{
			if (string.IsNullOrEmpty(response.Body))
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingJsonVariable(name), response.Request, response, name);
			}
			Regex regex = new Regex(string.Format("{0}\\s*:\\s*['\"](?<value>[^'\"]*)['\"]", Regex.Escape(name)));
			Match match = regex.Match(response.Body);
			if (!match.Success)
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingJsonVariable(name), response.Request, response, name);
			}
			return match.Result("${value}");
		}

		public static string ParseJavascriptStringVariable(HttpWebResponseWrapper response, string name)
		{
			if (string.IsNullOrEmpty(response.Body))
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingJavascriptEmptyBody(name), response.Request, response, name);
			}
			string result;
			if (!ParsingUtility.TryParseJavascriptStringVariable(response, name, out result))
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingJavascriptVariable(name), response.Request, response, name);
			}
			return result;
		}

		public static bool TryParseJavascriptStringVariable(HttpWebResponseWrapper response, string name, out string result)
		{
			if (string.IsNullOrEmpty(response.Body))
			{
				result = null;
				return false;
			}
			Regex regex = new Regex(string.Format("var {0}[\\s]*=[\\s]*['\"](?<value>[^'\"]*)['\"]", Regex.Escape(name)));
			Match match = regex.Match(response.Body);
			if (!match.Success)
			{
				result = null;
				return false;
			}
			result = match.Result("${value}");
			return true;
		}

		public static bool TryParseQueryParameter(Uri uri, string name, out string result)
		{
			if (uri == null)
			{
				result = null;
				return false;
			}
			Regex regex = new Regex(string.Format("{0}=(?<value>[^;]*)", Regex.Escape(name)));
			Match match = regex.Match(uri.Query);
			if (!match.Success)
			{
				result = null;
				return false;
			}
			result = match.Result("${value}");
			return true;
		}

		public static Dictionary<string, string> ParseHiddenFields(HttpWebResponseWrapper response)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (response.Body == null)
			{
				return dictionary;
			}
			Regex regex = new Regex("<input.*?type=[\"']hidden[\"'].*?name=[\"'](?<Name>[^\"']*)[\"'].*?value=[\"'](?<Value>[^\"']*)[\"'].*?[/]*>", RegexOptions.IgnoreCase);
			MatchCollection matchCollection = regex.Matches(response.Body);
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				dictionary.Add(match.Result("${Name}"), HttpUtility.HtmlDecode(match.Result("${Value}")));
			}
			return dictionary;
		}

		public static Dictionary<string, string> ParseInputFields(HttpWebResponseWrapper response)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (response.Body == null)
			{
				return dictionary;
			}
			Regex regex = new Regex("<input(.*?name=[\"'](?<Name1>[^\"']*)[\"'])?.*?type=[\"'](text|password|hidden|submit)[\"'].*?(.*?name=[\"'](?<Name2>[^\"']*)[\"'])?(.*?value=[\"'](?<Value>[^\"']*)[\"'].*?[/]*)?", RegexOptions.IgnoreCase);
			MatchCollection matchCollection = regex.Matches(response.Body);
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				string text = match.Result("${Name1}");
				if (string.IsNullOrEmpty(text))
				{
					text = match.Result("${Name2}");
				}
				string text2 = match.Result("${Value}");
				if (!string.IsNullOrEmpty(text2))
				{
					text2 = HttpUtility.HtmlDecode(text2);
				}
				dictionary.Add(text, text2);
			}
			return dictionary;
		}

		public static string ParseFormAction(HttpWebResponseWrapper response)
		{
			if (response.Body == null)
			{
				return null;
			}
			string regexString = "action[\\s]*=(?:(?:\\s*(?<value>[^'\"\\s]+)\\s)|[\\s]*[\"](?<value>[^\"]*)[\"]|[\\s]*['](?<value>[^']*)['])";
			return HttpUtility.HtmlDecode(ParsingUtility.ParseRegexResult(response, regexString, "${value}"));
		}

		public static string ParseFormDestination(HttpWebResponseWrapper response)
		{
			string regexString = "<input\\s+type=\"hidden\"\\s+name=\"destination\"\\s+value=\"(?<value>.*?)\">";
			return ParsingUtility.ParseRegexResult(response, regexString, "${value}");
		}

		public static string ParseFilePath(HttpWebResponseWrapper response, string fileName)
		{
			string regexString = string.Format("src=[\"'](?<path>[^\\s'\"]+?{0})[\"']", Regex.Escape(fileName));
			return ParsingUtility.ParseRegexResult(response, regexString, "${path}");
		}

		public static string ParseInnerHtml(HttpWebResponseWrapper response, string elementName, string elementId)
		{
			int num = ParsingUtility.FindOccurrence(response, "id=[\"']+" + elementId);
			if (num < 0)
			{
				return null;
			}
			int num2 = response.Body.IndexOf('>', num);
			if (num2 < 0)
			{
				return null;
			}
			int num3 = response.Body.IndexOf(string.Format("</{0}>", elementName), num2, StringComparison.InvariantCultureIgnoreCase);
			if (num3 < 0)
			{
				return null;
			}
			return response.Body.Substring(num2 + 1, num3 - num2);
		}

		public static string RemoveHtmlTags(string html)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in html)
			{
				if (c == '<')
				{
					flag = true;
					stringBuilder.Append(" ");
				}
				else if (c == '>')
				{
					flag = false;
				}
				else if (!flag)
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private static string ParseRegexResult(HttpWebResponseWrapper response, string regexString, string resultString)
		{
			Regex regex = new Regex(regexString, RegexOptions.Compiled);
			Match match = regex.Match(response.Body);
			if (!match.Success)
			{
				return null;
			}
			return match.Result(resultString);
		}

		private static int FindOccurrence(HttpWebResponseWrapper response, string regexString)
		{
			if (response.Body == null)
			{
				return -2;
			}
			Regex regex = new Regex(regexString, RegexOptions.Compiled);
			Match match = regex.Match(response.Body);
			if (!match.Success)
			{
				return -1;
			}
			return match.Index;
		}

		public static string JavascriptDecode(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s", "String to javascript decode can not be null");
			}
			StringBuilder sb = new StringBuilder();
			string result;
			using (StringWriter stringWriter = new StringWriter(sb))
			{
				for (int i = 0; i < s.Length; i++)
				{
					if (s[i] == '\\')
					{
						if (i + 1 < s.Length)
						{
							char c = s[i + 1];
							if (c <= '>')
							{
								if (c <= '\'')
								{
									switch (c)
									{
									case '!':
									case '"':
										break;
									default:
										if (c != '\'')
										{
											goto IL_130;
										}
										break;
									}
								}
								else if (c != '/')
								{
									switch (c)
									{
									case '<':
									case '>':
										break;
									case '=':
										goto IL_130;
									default:
										goto IL_130;
									}
								}
								stringWriter.Write(s[i + 1]);
								i++;
							}
							else if (c <= 'n')
							{
								if (c != '\\')
								{
									if (c == 'n')
									{
										stringWriter.Write('\n');
										i++;
									}
								}
								else
								{
									stringWriter.Write('\\');
									i++;
								}
							}
							else if (c != 'r')
							{
								if (c == 'u')
								{
									string s2 = s.Substring(i + 2, 4);
									int num = int.Parse(s2, NumberStyles.HexNumber);
									stringWriter.Write((char)num);
									i += 5;
								}
							}
							else
							{
								stringWriter.Write('\r');
								i++;
							}
						}
					}
					else
					{
						stringWriter.Write(s[i]);
					}
					IL_130:;
				}
				result = stringWriter.ToString();
			}
			return result;
		}
	}
}
