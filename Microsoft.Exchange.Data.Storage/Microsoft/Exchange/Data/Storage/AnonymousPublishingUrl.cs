using System;
using System.Collections.Specialized;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AnonymousPublishingUrl
	{
		public AnonymousPublishingUrl(Uri url)
		{
			Util.ThrowOnNullArgument(url, "url");
			this.Url = url;
			if (!AnonymousPublishingUrl.IsValidAnonymousPublishingUrl(this.Url))
			{
				throw new AnonymousPublishingUrlValidationException(this.Url.ToString());
			}
			this.Resource = this.Url.Segments[this.Url.Segments.Length - 1];
			this.QueryString = new NameValueCollection();
			if (this.Url.Query.Length > 0)
			{
				HttpRequest httpRequest = new HttpRequest(string.Empty, this.Url.ToString(), this.Url.Query.Substring(1));
				foreach (string name in httpRequest.QueryString.AllKeys)
				{
					this.QueryString[name] = httpRequest.QueryString[name];
				}
			}
			this.ParameterSegments = new string[this.Url.Segments.Length - 4];
			int num = 0;
			for (int j = 3; j < this.Url.Segments.Length - 1; j++)
			{
				this.ParameterSegments[num++] = this.Url.Segments[j].Substring(0, this.Url.Segments[j].Length - 1);
			}
		}

		public Uri Url { get; private set; }

		public string Resource { get; private set; }

		public NameValueCollection QueryString { get; private set; }

		public string[] ParameterSegments { get; private set; }

		public static bool IsValidAnonymousPublishingUrl(Uri urlToCheck)
		{
			return urlToCheck != null && urlToCheck.Segments.Length >= 4 && string.Equals(urlToCheck.Segments[0], AnonymousPublishingUrl.baseSegments[0], StringComparison.OrdinalIgnoreCase) && string.Equals(urlToCheck.Segments[1], AnonymousPublishingUrl.baseSegments[1], StringComparison.OrdinalIgnoreCase) && string.Equals(urlToCheck.Segments[2], AnonymousPublishingUrl.baseSegments[2], StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsValidBaseAnonymousPublishingUrl(Uri urlToCheck)
		{
			return urlToCheck != null && urlToCheck.Segments.Length == 3 && string.Equals(urlToCheck.Segments[0], AnonymousPublishingUrl.baseSegments[0], StringComparison.OrdinalIgnoreCase) && string.Equals(urlToCheck.Segments[1], AnonymousPublishingUrl.baseSegments[1], StringComparison.OrdinalIgnoreCase) && (string.Equals(urlToCheck.Segments[2], AnonymousPublishingUrl.baseSegments[2], StringComparison.OrdinalIgnoreCase) || string.Equals(urlToCheck.Segments[2], "calendar", StringComparison.OrdinalIgnoreCase));
		}

		public override string ToString()
		{
			return this.Url.OriginalString;
		}

		private const string OwaVdirName = "owa";

		private const string AnonymousVdirName = "calendar";

		private static readonly string[] baseSegments = new string[]
		{
			"/",
			"owa/",
			"calendar/"
		};
	}
}
