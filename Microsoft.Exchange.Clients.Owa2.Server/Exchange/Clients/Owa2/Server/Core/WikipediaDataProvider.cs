using System;
using System.Net.Http.Headers;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class WikipediaDataProvider : WebPageDataProvider
	{
		public WikipediaDataProvider(Uri uri, GetLinkPreviewRequest request, RequestDetailsLogger logger) : base(uri, request, logger)
		{
		}

		public override GetLinkPreviewResponse CreatePreview(DataProviderInformation dataProviderInformation)
		{
			LinkPreviewBuilder linkPreviewBuilder = new WikipediaLinkPreviewBuilder(this.request, ((WebPageInformation)dataProviderInformation).Text, this.logger, dataProviderInformation.ResponseUri);
			return linkPreviewBuilder.Execute();
		}

		protected override int GetMaxByteCount(Uri responseUri)
		{
			return 2048;
		}

		protected override void RequireContentType(MediaTypeHeaderValue contentType)
		{
			if (contentType == null || string.IsNullOrWhiteSpace(contentType.MediaType))
			{
				GetLinkPreview.ThrowInvalidRequestException("UnsupportedContentType", string.Format("Content type {0} is not supported.", "null"));
			}
			string text = contentType.MediaType.ToLower();
			if (!text.Contains("text/xml"))
			{
				GetLinkPreview.ThrowInvalidRequestException("UnsupportedContentType", string.Format("Content type {0} is not supported.", text));
			}
		}

		protected new const int MaxByteCount = 2048;
	}
}
