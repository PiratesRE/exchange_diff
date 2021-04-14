using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CraigsListLinkPreviewBuilder : WebPageLinkPreviewBuilder
	{
		public CraigsListLinkPreviewBuilder(GetLinkPreviewRequest request, string responseString, RequestDetailsLogger logger, Uri responseUri) : base(request, responseString, logger, responseUri, false)
		{
		}

		protected override string GetDescription(out int descriptionTagCount)
		{
			string text = null;
			Match match = LinkPreviewBuilder.ExecuteRegEx(CraigsListLinkPreviewBuilder.GetDescriptionRegEx, this.responseString, "description");
			descriptionTagCount = match.Groups["description"].Captures.Count;
			if (descriptionTagCount > 0)
			{
				text = LinkPreviewBuilder.ConvertToSafeHtml(match.Groups["description"].Value);
				text = WebPageLinkPreviewBuilder.ReplaceSelectedHtmlEntities(text);
			}
			this.logger.Set(GetLinkPreviewMetadata.DescriptionLength, WebPageLinkPreviewBuilder.GetStringLength(text));
			return WebPageLinkPreviewBuilder.Truncate(text, 1000);
		}

		protected override string GetImage(out int imageTagCount)
		{
			string imageUrl = null;
			Match match = LinkPreviewBuilder.ExecuteRegEx(CraigsListLinkPreviewBuilder.GetImageSrcRegEx, this.responseString, "image");
			imageTagCount = match.Groups["imageUrl"].Captures.Count;
			if (imageTagCount > 0)
			{
				imageUrl = LinkPreviewBuilder.ConvertToSafeHtml(match.Groups["imageUrl"].Value);
			}
			return base.GetImageUrlAbsolutePath(imageUrl);
		}

		public static bool IsCraigsListUri(Uri responseUri)
		{
			return responseUri.Host != null && responseUri.Host.ToUpper().Contains(".CRAIGSLIST.");
		}

		private const string CraigsListHostSegmentUpperCase = ".CRAIGSLIST.";

		private const string DescriptionRegExKey = "description";

		private const string ImageSrcRegExKey = "imageUrl";

		private const string DescriptionRegEx = "<section id=('|\")postingbody('|\")>(?<description>.*?)</section>";

		private const string ImageSrcRegEx = "<img\\sid=('|\")iwi('|\")\\ssrc=('|\")(?<imageUrl>.*?)('|\")[^><]*?title=('|\")image 1('|\")[^><]*?>";

		private static Regex GetDescriptionRegEx = new Regex("<section id=('|\")postingbody('|\")>(?<description>.*?)</section>", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);

		private static Regex GetImageSrcRegEx = new Regex("<img\\sid=('|\")iwi('|\")\\ssrc=('|\")(?<imageUrl>.*?)('|\")[^><]*?title=('|\")image 1('|\")[^><]*?>", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);
	}
}
