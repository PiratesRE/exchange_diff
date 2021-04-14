using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class OwaPlainTextStyle
	{
		private static Dictionary<string, string> CreateHtmlStyleTable()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["windows-1256"] = " { font-size:120%; font-family:monospace; }";
			dictionary["iso-8859-6"] = " { font-size:120%; font-family:monospace;  }";
			dictionary["asmo-708"] = " { font-size:120%; font-family:monospace; }";
			dictionary["windows-1255"] = " { font-size:120%; font-family:monospace; }";
			dictionary["iso-8859-8"] = " { font-size:120%; font-family:monospace; }";
			dictionary["iso-8859-8-i"] = " { font-size:120%; font-family:monospace; }";
			dictionary["windows-1250"] = " { font-size:120%; font-family:monospace; }";
			dictionary["windows-1252"] = " { font-size:120%; font-family:monospace; }";
			dictionary["windows-1254"] = " { font-size:120%; font-family:monospace; }";
			dictionary["windows-1257"] = " { font-size:120%; font-family:monospace; }";
			dictionary["iso-8859-1"] = " { font-size:120%; font-family:monospace; }";
			dictionary["iso-8859-2"] = " { font-size:120%; font-family:monospace; }";
			dictionary["iso-8859-4"] = " { font-size:120%; font-family:monospace; }";
			dictionary["iso-8859-9"] = " { font-size:120%; font-family:monospace; }";
			dictionary["iso-8859-15"] = " { font-size:120%; font-family:monospace; }";
			dictionary["us-ascii"] = " { font-size:120%; font-family:monospace; }";
			dictionary["utf-8"] = " { font-size:120%; font-family:monospace; }";
			dictionary["iso-8859-5"] = " { font-size:110%; font-family:monospace; }";
			dictionary["iso-8859-7"] = " { font-size:110%; font-family:monospace; }";
			dictionary["windows-874"] = " { font-size:110%; font-family:monospace; }";
			dictionary["windows-1253"] = " { font-size:110%; font-family:monospace; }";
			dictionary["koi8-r"] = " { font-size:110%; font-family:monospace; }";
			dictionary["koi8-u"] = " { font-size:110%; font-family:monospace; }";
			dictionary["gb2312"] = " { font-size:110%; font-family:SimSun-18030,SimSun,monospace; }";
			dictionary["gb18030"] = " { font-size:110%; font-family:SimSun-18030,SimSun,monospace; }";
			dictionary["euc-cn"] = " { font-size:110%; font-family:SimSun-18030,SimSun,monospace; }";
			dictionary["hz-gb-2132"] = " { font-size:110%; font-family:SimSun-18030,SimSun,monospace; }";
			dictionary["big5"] = " { font-size:110%; font-family:MingLiu,monospace;  }";
			dictionary["iso-2022-jp"] = " { font-size:110%; font-family:\"MS-Gothic\",monospace;  }";
			dictionary["euc-jp"] = " { font-size:110%; font-family:\"MS-Gothic\",monospace;  }";
			dictionary["shift_jis"] = " { font-size:110%; font-family:\"MS-Gothic\",monospace;  }";
			dictionary["ks_c_5601-1987"] = " { font-size:110%; font-family:GulimChe,monospace;  }";
			dictionary["euc-kr"] = " { font-size:110%; font-family:GulimChe,monospace;  }";
			dictionary["iso-2022-kr"] = " { font-size:110%; font-family:GulimChe,monospace;  }";
			return dictionary;
		}

		public static void WriteLocalizedStyleIntoHeadForPlainTextBody(Item item, TextWriter writer, string styleElement)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (styleElement == null)
			{
				throw new ArgumentNullException("styleElement");
			}
			if (item.Body.Format == BodyFormat.TextPlain)
			{
				writer.Write("<style>");
				writer.Write(styleElement);
				writer.Write(OwaPlainTextStyle.GetStyleFromCharset(item));
				writer.Write("</style>");
			}
		}

		private static string GetStyleFromCharset(Item item)
		{
			return OwaPlainTextStyle.GetStyleFromCharset(item.Body.Charset);
		}

		internal static string GetStyleFromCharset(string charset)
		{
			string text = null;
			OwaPlainTextStyle.htmlStyleTable.TryGetValue(charset.ToLowerInvariant(), out text);
			if (string.IsNullOrEmpty(text))
			{
				return "{font-family:monospace;}";
			}
			return text;
		}

		internal static string GetStyleContentFromUserOption(UserOptions options, bool needJavascriptEncode)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			StringBuilder stringBuilder = new StringBuilder(150);
			stringBuilder.Append("font-family:");
			if (needJavascriptEncode)
			{
				stringBuilder.Append(Utilities.JavascriptEncode(options.ComposeFontName));
			}
			else
			{
				stringBuilder.Append(options.ComposeFontName);
			}
			stringBuilder.Append("; ");
			stringBuilder.Append("font-size:");
			stringBuilder.Append(Utilities.ConvertFromFontSize(options.ComposeFontSize));
			stringBuilder.Append("pt; ");
			if (Utilities.IsFlagSet((int)options.ComposeFontFlags, 1))
			{
				stringBuilder.Append("font-weight:bold; ");
			}
			if (Utilities.IsFlagSet((int)options.ComposeFontFlags, 2))
			{
				stringBuilder.Append("font-style:italic; ");
			}
			if (Utilities.IsFlagSet((int)options.ComposeFontFlags, 4))
			{
				stringBuilder.Append("text-decoration:underline; ");
			}
			return stringBuilder.ToString();
		}

		internal static string GetStyleFromUserOption(UserOptions options)
		{
			return '{' + OwaPlainTextStyle.GetStyleContentFromUserOption(options, false) + '}';
		}

		private const string DefaultStyle = "{font-family:monospace;}";

		private static Dictionary<string, string> htmlStyleTable = OwaPlainTextStyle.CreateHtmlStyleTable();
	}
}
