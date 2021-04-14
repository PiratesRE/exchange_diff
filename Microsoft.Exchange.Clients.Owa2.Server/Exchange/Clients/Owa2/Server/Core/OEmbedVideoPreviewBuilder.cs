using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OEmbedVideoPreviewBuilder : WebPageLinkPreviewBuilder
	{
		public OEmbedVideoPreviewBuilder(GetLinkPreviewRequest request, string responseString, OEmbedResponse oEmbedResponse, RequestDetailsLogger logger, Uri responseUri) : base(request, responseString, logger, responseUri, true)
		{
			this.oEmbedResponse = oEmbedResponse;
		}

		private static OEmbedVideoPreviewBuilder.OEmbedProviderDetails[] OEmbedProviderDetailsArray
		{
			get
			{
				if (OEmbedVideoPreviewBuilder.oembedProviderDetails == null)
				{
					OEmbedVideoPreviewBuilder.oembedProviderDetails = new OEmbedVideoPreviewBuilder.OEmbedProviderDetails[3];
					OEmbedVideoPreviewBuilder.oembedProviderDetails[0] = new OEmbedVideoPreviewBuilder.OEmbedProviderDetails();
					OEmbedVideoPreviewBuilder.oembedProviderDetails[0].UrlScheme = "https?://(?:www\\.)?dailymotion\\.com/video.*";
					OEmbedVideoPreviewBuilder.oembedProviderDetails[0].ApiEndPoint = "https://www.dailymotion.com/services/oembed?format=json&url={0}&maxwidth=640&maxheight=480";
					OEmbedVideoPreviewBuilder.oembedProviderDetails[1] = new OEmbedVideoPreviewBuilder.OEmbedProviderDetails();
					OEmbedVideoPreviewBuilder.oembedProviderDetails[1].UrlScheme = "https?://(?:www\\.)?hulu\\.com/watch.*";
					OEmbedVideoPreviewBuilder.oembedProviderDetails[1].ApiEndPoint = "https://secure.hulu.com/api/oembed.json?url={0}&maxwidth=640&maxheight=480";
					OEmbedVideoPreviewBuilder.oembedProviderDetails[2] = new OEmbedVideoPreviewBuilder.OEmbedProviderDetails();
					OEmbedVideoPreviewBuilder.oembedProviderDetails[2].UrlScheme = "https?://(?:www\\.)?vimeo\\.com.*";
					OEmbedVideoPreviewBuilder.oembedProviderDetails[2].ApiEndPoint = "https://vimeo.com/api/oembed.json?url={0}&maxwidth=640&maxheight=480";
				}
				return OEmbedVideoPreviewBuilder.oembedProviderDetails;
			}
		}

		internal static bool IsOEmbedVideoUri(Uri uri, RequestDetailsLogger logger)
		{
			foreach (OEmbedVideoPreviewBuilder.OEmbedProviderDetails oembedProviderDetails in OEmbedVideoPreviewBuilder.OEmbedProviderDetailsArray)
			{
				if (oembedProviderDetails.UrlSchemeRegex.IsMatch(uri.AbsoluteUri))
				{
					return true;
				}
			}
			return false;
		}

		internal static string GetOEmbedQueryForUri(Uri uri)
		{
			foreach (OEmbedVideoPreviewBuilder.OEmbedProviderDetails oembedProviderDetails in OEmbedVideoPreviewBuilder.OEmbedProviderDetailsArray)
			{
				if (oembedProviderDetails.UrlSchemeRegex.IsMatch(uri.AbsoluteUri))
				{
					return string.Format(oembedProviderDetails.ApiEndPoint, uri.AbsoluteUri);
				}
			}
			return null;
		}

		protected override LinkPreview CreateLinkPreviewInstance()
		{
			return new OEmbedVideoPreview();
		}

		protected override void SetAdditionalProperties(LinkPreview linkPreview)
		{
			if (this.oEmbedResponse == null)
			{
				GetLinkPreview.ThrowInvalidRequestException("OEmbedResponseNull", string.Format("The OEmbedResponse was null for the webpage information for {0}", this.responseUri.AbsoluteUri));
			}
			if (this.oEmbedResponse.Html == null)
			{
				GetLinkPreview.ThrowInvalidRequestException("OEmbedResponseHtmlNull", string.Format("The OEmbedResponse HTML was null for the webpage information for {0}", this.responseUri.AbsoluteUri));
			}
			string text = this.oEmbedResponse.Html;
			if (this.oEmbedResponse.ProviderName.Equals("YouTube") || this.oEmbedResponse.ProviderName.Equals("Dailymotion"))
			{
				text = text.Replace("http://", "https://");
			}
			else if (this.oEmbedResponse.ProviderName.Equals("Hulu"))
			{
				text = text.Replace("http://www.hulu.com", "https://secure.hulu.com");
			}
			((OEmbedVideoPreview)linkPreview).EmbeddedHtml = text;
		}

		protected override string GetImage(out int imageTagCount)
		{
			string thumbnailUrl = this.oEmbedResponse.ThumbnailUrl;
			if (thumbnailUrl != null)
			{
				if (thumbnailUrl.Length > 500)
				{
					GetLinkPreview.ThrowInvalidRequestException("MaxImageUrlLengthExceeded", string.Format("Image url length {0} exceeds the maximum length allowed.", thumbnailUrl.Length));
				}
				imageTagCount = 1;
				return thumbnailUrl;
			}
			imageTagCount = 0;
			return null;
		}

		protected override string GetTitle()
		{
			string text = this.oEmbedResponse.Title;
			if (text != null)
			{
				text = LinkPreviewBuilder.ConvertToSafeHtml(text);
				text = WebPageLinkPreviewBuilder.ReplaceSelectedHtmlEntities(text);
				return WebPageLinkPreviewBuilder.Truncate(text, 400);
			}
			return null;
		}

		protected override string GetDescription(out int descriptionTagCount)
		{
			if (this.responseString == null)
			{
				descriptionTagCount = 0;
				return null;
			}
			return base.GetDescription(out descriptionTagCount);
		}

		private const string VideoWidthHeightQueryString = "&maxwidth=640&maxheight=480";

		private const string UrlPrefixRegex = "https?://(?:www\\.)?";

		private static OEmbedVideoPreviewBuilder.OEmbedProviderDetails[] oembedProviderDetails;

		private readonly OEmbedResponse oEmbedResponse;

		private class OEmbedProviderDetails
		{
			internal string UrlScheme { private get; set; }

			internal string ApiEndPoint { get; set; }

			internal Regex UrlSchemeRegex
			{
				get
				{
					if (this.urlSchemeRegex == null)
					{
						this.urlSchemeRegex = new Regex(this.UrlScheme, WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);
					}
					return this.urlSchemeRegex;
				}
			}

			private Regex urlSchemeRegex;
		}
	}
}
