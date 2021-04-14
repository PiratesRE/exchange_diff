using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AmazonLinkPreviewBuilder : WebPageLinkPreviewBuilder
	{
		public AmazonLinkPreviewBuilder(GetLinkPreviewRequest request, string responseString, RequestDetailsLogger logger, Uri responseUri) : base(request, responseString, logger, responseUri, false)
		{
		}

		protected override string GetImage(out int imageTagCount)
		{
			string attributeValue = base.GetAttributeValue(this.responseString, AmazonLinkPreviewBuilder.GetImageTagRegEx, "imageTag", AmazonLinkPreviewBuilder.GetImageAttributeRegEx, "image", "image", out imageTagCount);
			return base.GetImageUrlAbsolutePath(attributeValue);
		}

		public static bool IsAmazonUri(Uri responseUri)
		{
			return responseUri.Host.ToUpper().StartsWith("WWW.AMAZON.");
		}

		private const string AmazonHostPrefixUpperCase = "WWW.AMAZON.";

		private const string ImageTagRegExKey = "imageTag";

		private const string ImageAttributeRegExKey = "image";

		private const string ImageTagRegEx = "<img(?<imageTag>[^><]*?\\sid=('|\")(imgBlkFront|main-image|landingImage|prod-img|prodImage)\\1[^><]*?)>";

		private const string ImageAttributeRegEx = "\\ssrc=('|\")(?<image>.*?)\\1";

		private static Regex GetImageTagRegEx = new Regex("<img(?<imageTag>[^><]*?\\sid=('|\")(imgBlkFront|main-image|landingImage|prod-img|prodImage)\\1[^><]*?)>", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);

		private static Regex GetImageAttributeRegEx = new Regex("\\ssrc=('|\")(?<image>.*?)\\1", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);
	}
}
