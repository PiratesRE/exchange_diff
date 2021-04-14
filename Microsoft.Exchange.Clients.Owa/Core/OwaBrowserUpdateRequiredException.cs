using System;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	internal class OwaBrowserUpdateRequiredException : OwaPermanentException
	{
		public OwaBrowserUpdateRequiredException(BrowserPlatform browserPlatform) : base(null)
		{
			this.browserPlatform = browserPlatform;
		}

		public string GetErrorDetails()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(729144936));
			stringBuilder.Append("<br><br>");
			if (this.browserPlatform == BrowserPlatform.Windows)
			{
				OwaBrowserUpdateRequiredException.AppendBrowserLink(stringBuilder, "http://microsoft.com/ie", 76632944);
				OwaBrowserUpdateRequiredException.AppendAlternateSuggestion(stringBuilder);
				OwaBrowserUpdateRequiredException.AppendBrowserLink(stringBuilder, "http://mozilla.org/firefox", 951275809);
				stringBuilder.Append("<br>");
				OwaBrowserUpdateRequiredException.AppendBrowserLink(stringBuilder, "http://www.google.com/chrome", 1899309595);
			}
			else if (this.browserPlatform == BrowserPlatform.Macintosh)
			{
				OwaBrowserUpdateRequiredException.AppendBrowserLink(stringBuilder, "http://www.apple.com/safari/", 2140109750);
				OwaBrowserUpdateRequiredException.AppendAlternateSuggestion(stringBuilder);
				OwaBrowserUpdateRequiredException.AppendBrowserLink(stringBuilder, "http://mozilla.org/firefox", 951275809);
			}
			else
			{
				OwaBrowserUpdateRequiredException.AppendBrowserLink(stringBuilder, "http://mozilla.org/firefox", 2093055582);
			}
			return stringBuilder.ToString();
		}

		private static void AppendBrowserLink(StringBuilder errorDetails, string linkValue, Strings.IDs linkText)
		{
			errorDetails.Append("<a href=\"");
			errorDetails.Append(linkValue);
			errorDetails.Append("\">");
			errorDetails.Append(LocalizedStrings.GetHtmlEncoded(linkText));
			errorDetails.Append("</a>");
		}

		private static void AppendAlternateSuggestion(StringBuilder errorDetails)
		{
			errorDetails.Append("<br><br><br>");
			errorDetails.Append(LocalizedStrings.GetHtmlEncoded(427833413));
			errorDetails.Append("<br><br>");
		}

		private const string DownloadLocationIE = "http://microsoft.com/ie";

		private const string DownloadLocationSafari = "http://www.apple.com/safari/";

		private const string DownloadLocationFireFox = "http://mozilla.org/firefox";

		private const string DownloadLocationChrome = "http://www.google.com/chrome";

		private BrowserPlatform browserPlatform;
	}
}
