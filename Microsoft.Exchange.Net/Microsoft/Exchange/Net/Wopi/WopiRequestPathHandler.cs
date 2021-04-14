using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.Net.Wopi
{
	public static class WopiRequestPathHandler
	{
		public static bool IsWopiRequest(HttpRequest request, bool isFrontend)
		{
			string text = request.Url.AbsolutePath;
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			text = HttpUtility.UrlDecode(text);
			if (text.IndexOf("/wopi/", StringComparison.OrdinalIgnoreCase) < 0)
			{
				return false;
			}
			WopiRequestPathHandler.EnsureRegexInit(isFrontend);
			bool flag = WopiRequestPathHandler.wopiUrlRegex.IsMatch(text);
			if (!flag)
			{
				ExTraceGlobals.VerboseTracer.TraceWarning<string, Uri>((long)typeof(WopiRequestPathHandler).GetHashCode(), "[WopiRequestMatcher::IsWopiRequest]: Method {0}; Url {1}; Matched /wopi/ but regex match failed.", request.HttpMethod, request.Url);
			}
			return flag;
		}

		public static string GetUserEmailAddress(HttpRequest request)
		{
			string text = request.Url.AbsolutePath;
			text = HttpUtility.UrlDecode(text);
			WopiRequestPathHandler.EnsureRegexInit(false);
			Match match = WopiRequestPathHandler.wopiUrlRegex.Match(text);
			string value = match.Groups[1].Value;
			return value.Trim();
		}

		public static string StripEmailAddress(string path, string emailAddress)
		{
			string str = path.Substring(0, 5);
			string str2 = path.Substring(5 + (emailAddress.Length + 1));
			return str + str2;
		}

		private static void EnsureRegexInit(bool isFrontEnd)
		{
			if (WopiRequestPathHandler.wopiUrlRegex == null)
			{
				lock (WopiRequestPathHandler.Monitor)
				{
					if (WopiRequestPathHandler.wopiUrlRegex == null)
					{
						if (isFrontEnd)
						{
							WopiRequestPathHandler.wopiUrlRegex = new Regex(WopiRequestPathHandler.WopiUrlRegexPatternFrontend, RegexOptions.IgnoreCase | RegexOptions.Compiled);
						}
						else if (WopiRequestPathHandler.IsRunningDfpowa)
						{
							WopiRequestPathHandler.wopiUrlRegex = new Regex(WopiRequestPathHandler.DfpowaWopiUrlRegexPatternBackend, RegexOptions.IgnoreCase | RegexOptions.Compiled);
						}
						else
						{
							WopiRequestPathHandler.wopiUrlRegex = new Regex(WopiRequestPathHandler.WopiUrlRegexPatternBackend, RegexOptions.IgnoreCase | RegexOptions.Compiled);
						}
					}
				}
			}
		}

		private static readonly bool IsRunningDfpowa = !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IsPreCheckinApp"]) && StringComparer.OrdinalIgnoreCase.Equals("true", ConfigurationManager.AppSettings["IsPreCheckinApp"]);

		private static readonly object Monitor = new object();

		private static readonly string WopiUrlRegexPatternFrontend = "^/owa/(.+?@.+?)/wopi/files/@/owaatt(/contents)?$";

		private static readonly string WopiUrlRegexPatternBackend = "^/owa/wopi/files/@/owaatt(/contents)?$";

		private static readonly string DfpowaWopiUrlRegexPatternBackend = "^/dfpowa[1-5]?/wopi/files/@/owaatt(/contents)?$";

		private static Regex wopiUrlRegex;
	}
}
