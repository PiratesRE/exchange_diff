using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class WikipediaLinkPreviewBuilder : WebPageLinkPreviewBuilder
	{
		public WikipediaLinkPreviewBuilder(GetLinkPreviewRequest request, string responseString, RequestDetailsLogger logger, Uri responseUri) : base(request, responseString, logger, responseUri, false)
		{
		}

		protected override string GetImage(out int imageTagCount)
		{
			imageTagCount = 0;
			return null;
		}

		protected override string GetTitle()
		{
			string text = null;
			Match match = LinkPreviewBuilder.ExecuteRegEx(WikipediaLinkPreviewBuilder.GetTitleRegEx, this.responseString, "title");
			if (match.Groups["title"].Captures.Count > 0)
			{
				text = LinkPreviewBuilder.ConvertToSafeHtml(match.Groups["title"].Value);
			}
			this.logger.Set(GetLinkPreviewMetadata.TitleLength, WebPageLinkPreviewBuilder.GetStringLength(text));
			return WebPageLinkPreviewBuilder.Truncate(text, 400);
		}

		protected override string GetDescription(out int descriptionTagCount)
		{
			string text = null;
			descriptionTagCount = 0;
			Match match = LinkPreviewBuilder.ExecuteRegEx(WikipediaLinkPreviewBuilder.GetDescriptionRegEx, this.responseString, "title");
			if (match.Groups["description"].Captures.Count > 0)
			{
				text = LinkPreviewBuilder.ConvertToSafeHtml(match.Groups["description"].Value);
				descriptionTagCount = 1;
			}
			this.logger.Set(GetLinkPreviewMetadata.DescriptionLength, WebPageLinkPreviewBuilder.GetStringLength(text));
			return WebPageLinkPreviewBuilder.Truncate(text, 1000);
		}

		public static bool IsWikipediaUri(Uri uri)
		{
			return uri.Host != null && uri.Host.ToUpper().EndsWith(".WIKIPEDIA.ORG");
		}

		public static bool TryGetWikipediaServiceUri(Uri uri, out Uri wikipediaServiceUri)
		{
			wikipediaServiceUri = null;
			if (WikipediaLinkPreviewBuilder.IsWikipediaUri(uri))
			{
				string wikipediaServiceUrl = WikipediaLinkPreviewBuilder.GetWikipediaServiceUrl(uri);
				if (wikipediaServiceUrl != null)
				{
					wikipediaServiceUri = new Uri(wikipediaServiceUrl);
				}
			}
			return wikipediaServiceUri != null;
		}

		private static string GetWikipediaServiceUrl(Uri uri)
		{
			string result = null;
			if ((uri.Host.Length == WikipediaLinkPreviewBuilder.DesktopHostLength || uri.Host.Length == WikipediaLinkPreviewBuilder.MobileHostLength) && uri.AbsolutePath.ToUpper().StartsWith("/WIKI/") && uri.Segments.Length == 3)
			{
				result = string.Format("http://{0}/w/api.php?action=query&prop=extracts|info&titles={1}&exintro=1&explaintext=1&exchars=150&inprop=displaytitle&format=xml", uri.Host, uri.Segments[2]);
			}
			return result;
		}

		private const string HostSuffixUpperCase = ".WIKIPEDIA.ORG";

		private const int HostLocaleLength = 2;

		private const int HostMobileLength = 1;

		private const int HostDelimiterLength = 1;

		private const int WikipediaDesktopSegmentsLength = 3;

		private const int WikipediaMobileSegmentsLength = 4;

		private const string WikipediaPathPrefixUpperCase = "/WIKI/";

		private const string WikipediaServiceFormatString = "http://{0}/w/api.php?action=query&prop=extracts|info&titles={1}&exintro=1&explaintext=1&exchars=150&inprop=displaytitle&format=xml";

		private const string DescriptionCharacterLength = "150";

		private const string TitleRegExKey = "title";

		private const string DescriptionRegExKey = "description";

		private const string TitleRegEx = "<page\\s[^<>]*?title=('|\")(?<title>.*?)('|\")[^<>]*?>";

		private const string DescriptionRegEx = "<extract[^<>]*?>(?<description>.*?)</extract>";

		private static readonly int DesktopHostLength = 2 + ".WIKIPEDIA.ORG".Length;

		private static readonly int MobileHostLength = 4 + ".WIKIPEDIA.ORG".Length;

		private static Regex GetTitleRegEx = new Regex("<page\\s[^<>]*?title=('|\")(?<title>.*?)('|\")[^<>]*?>", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);

		private static Regex GetDescriptionRegEx = new Regex("<extract[^<>]*?>(?<description>.*?)</extract>", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);
	}
}
