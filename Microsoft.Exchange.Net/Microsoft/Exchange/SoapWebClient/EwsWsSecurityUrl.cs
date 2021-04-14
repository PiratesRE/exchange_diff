using System;

namespace Microsoft.Exchange.SoapWebClient
{
	internal static class EwsWsSecurityUrl
	{
		public static bool IsWsSecurity(string url)
		{
			return url.EndsWith("/WSSecurity", StringComparison.OrdinalIgnoreCase) || url.EndsWith("/WSSecurity/", StringComparison.OrdinalIgnoreCase);
		}

		public static string Fix(string url)
		{
			if (EwsWsSecurityUrl.IsWsSecurity(url))
			{
				return url;
			}
			return EwsWsSecurityUrl.AppendSuffix(url);
		}

		public static Uri Fix(Uri url)
		{
			if (EwsWsSecurityUrl.IsWsSecurity(url.OriginalString))
			{
				return url;
			}
			return new Uri(EwsWsSecurityUrl.AppendSuffix(url.OriginalString));
		}

		public static string FixForAnonymous(string url)
		{
			if (!EwsWsSecurityUrl.IsWsSecurity(url))
			{
				return url;
			}
			return EwsWsSecurityUrl.RemoveSuffix(url);
		}

		public static Uri FixForAnonymous(Uri url)
		{
			if (!EwsWsSecurityUrl.IsWsSecurity(url.OriginalString))
			{
				return url;
			}
			return new Uri(EwsWsSecurityUrl.RemoveSuffix(url.OriginalString));
		}

		private static string RemoveSuffix(string url)
		{
			if (url.EndsWith("/WSSecurity", StringComparison.OrdinalIgnoreCase))
			{
				return url.Remove(url.Length - "/WSSecurity".Length, "/WSSecurity".Length);
			}
			return url.Remove(url.Length - "/WSSecurity/".Length, "/WSSecurity/".Length);
		}

		private static string AppendSuffix(string url)
		{
			if (url.EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				return url + "WSSecurity";
			}
			return url + "/WSSecurity";
		}

		private const string Slash = "/";

		private const string Suffix = "WSSecurity";

		private const string SlashSuffix = "/WSSecurity";

		private const string SlashSuffixSlash = "/WSSecurity/";
	}
}
