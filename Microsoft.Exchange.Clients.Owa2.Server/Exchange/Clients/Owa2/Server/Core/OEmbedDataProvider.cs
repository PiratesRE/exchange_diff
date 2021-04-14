using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OEmbedDataProvider : LinkPreviewDataProvider
	{
		public OEmbedDataProvider(Uri uri, GetLinkPreviewRequest request, RequestDetailsLogger logger) : base(uri, request, logger)
		{
		}

		protected override async Task<DataProviderInformation> GetDataProviderInformation(HttpClient httpClient)
		{
			DataProviderInformation result;
			if (this.request.Id.StartsWith("UpdateOEmbed"))
			{
				result = await base.MakeAndProcessHttpRequest(httpClient, OEmbedDataProvider.GetOEmbedRequestUri(this.uri), new LinkPreviewDataProvider.ProcessResponseStreamDelegate(OEmbedDataProvider.ProcessOEmbedResponseStream));
			}
			else
			{
				WebPageInformation webPageInformation = (WebPageInformation)(await base.MakeAndProcessHttpRequest(httpClient, this.uri, new LinkPreviewDataProvider.ProcessResponseStreamDelegate(WebPageDataProvider.ProcessResponseStream)));
				OEmbedInformation oembedInformation = (OEmbedInformation)(await base.MakeAndProcessHttpRequest(httpClient, OEmbedDataProvider.GetOEmbedRequestUri(this.uri), new LinkPreviewDataProvider.ProcessResponseStreamDelegate(OEmbedDataProvider.ProcessOEmbedResponseStream)));
				oembedInformation.Text = webPageInformation.Text;
				result = oembedInformation;
			}
			return result;
		}

		private static Uri GetOEmbedRequestUri(Uri uri)
		{
			string oembedQueryForUri = OEmbedVideoPreviewBuilder.GetOEmbedQueryForUri(uri);
			if (oembedQueryForUri != null)
			{
				return new Uri(oembedQueryForUri);
			}
			GetLinkPreview.ThrowInvalidRequestException("OEmbedQueryStringNotFound", string.Format("Could not get OEmbed query string for url {0}", uri.AbsoluteUri));
			return null;
		}

		protected static DataProviderInformation ProcessOEmbedResponseStream(Uri responseUri, Encoding responseHeaderEncoding, MemoryStream memoryStream, RequestDetailsLogger logger)
		{
			memoryStream.Position = 0L;
			OEmbedResponse oembedResponse = null;
			try
			{
				DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(OEmbedResponse));
				oembedResponse = (OEmbedResponse)dataContractJsonSerializer.ReadObject(memoryStream);
			}
			catch (SerializationException ex)
			{
				GetLinkPreview.ThrowInvalidRequestException("OEmbedResponseSerializationReadFailed", string.Format("Failed to read the OEmbed response object. Error {0}", ex.Message));
			}
			return new OEmbedInformation
			{
				Text = null,
				ResponseUri = responseUri,
				OEmbedResponse = oembedResponse
			};
		}

		protected override void RequireContentType(MediaTypeHeaderValue contentType)
		{
			if (contentType == null || string.IsNullOrWhiteSpace(contentType.MediaType))
			{
				GetLinkPreview.ThrowInvalidRequestException("UnsupportedContentType", string.Format("Content type {0} is not supported.", "null"));
			}
			string text = contentType.MediaType.ToLower();
			if (!text.Contains("text/html") && !text.Contains("application/xhtml+xml") && !text.Contains("application/json"))
			{
				GetLinkPreview.ThrowInvalidRequestException("UnsupportedContentType", string.Format("Content type {0} is not supported.", text));
			}
		}

		public override GetLinkPreviewResponse CreatePreview(DataProviderInformation dataProviderInformation)
		{
			OEmbedInformation oembedInformation = (OEmbedInformation)dataProviderInformation;
			string text = oembedInformation.Text;
			OEmbedResponse oembedResponse = oembedInformation.OEmbedResponse;
			LinkPreviewBuilder linkPreviewBuilder = new OEmbedVideoPreviewBuilder(this.request, text, oembedResponse, this.logger, dataProviderInformation.ResponseUri);
			return linkPreviewBuilder.Execute();
		}

		protected override void RestrictContentLength(long? contentLength)
		{
			if (contentLength != null && contentLength.Value > 524288L)
			{
				GetLinkPreview.ThrowInvalidRequestException("MaxContentLengthExceeded", string.Format("Content length {0} exceeds maximum size allowed.", contentLength.Value));
			}
		}

		protected override int GetMaxByteCount(Uri responseUri)
		{
			return 32768;
		}

		private const string JsonContentType = "application/json";

		private const string OEmbedRequestIdPrefix = "UpdateOEmbed";
	}
}
