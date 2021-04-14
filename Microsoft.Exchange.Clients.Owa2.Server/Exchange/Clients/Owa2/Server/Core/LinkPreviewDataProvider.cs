using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class LinkPreviewDataProvider
	{
		public LinkPreviewDataProvider(Uri uri, GetLinkPreviewRequest request, RequestDetailsLogger logger)
		{
			this.uri = uri;
			this.request = request;
			this.logger = logger;
		}

		public long ContentLength { get; set; }

		public abstract GetLinkPreviewResponse CreatePreview(DataProviderInformation dataProviderInformation);

		protected abstract Task<DataProviderInformation> GetDataProviderInformation(HttpClient httpClient);

		protected abstract void RequireContentType(MediaTypeHeaderValue contentType);

		protected abstract void RestrictContentLength(long? contentLength);

		protected abstract int GetMaxByteCount(Uri responseUri);

		public async Task<DataProviderInformation> GetDataProviderInformation()
		{
			DataProviderInformation result;
			using (HttpClientHandler httpClientHandler = new HttpClientHandler())
			{
				httpClientHandler.CookieContainer = new CookieContainer();
				using (HttpClient httpClient = new HttpClient(httpClientHandler))
				{
					httpClient.Timeout = TimeSpan.FromMilliseconds(6000.0);
					httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
					httpClient.DefaultRequestHeaders.Add("accept", "text/html, application/xhtml+xml, */*");
					result = await this.GetDataProviderInformation(httpClient);
				}
			}
			return result;
		}

		public static LinkPreviewDataProvider GetDataProvider(GetLinkPreviewRequest request, RequestDetailsLogger logger, bool activeViewsConvergenceEnabled)
		{
			Uri uri = LinkPreviewDataProvider.CreateUri(request.Url);
			LinkPreviewDataProvider result;
			Uri uri2;
			if (activeViewsConvergenceEnabled && OEmbedVideoPreviewBuilder.IsOEmbedVideoUri(uri, logger))
			{
				result = new OEmbedDataProvider(uri, request, logger);
			}
			else if (WikipediaLinkPreviewBuilder.TryGetWikipediaServiceUri(uri, out uri2))
			{
				result = new WikipediaDataProvider(uri2, request, logger);
			}
			else
			{
				result = new WebPageDataProvider(uri, request, logger);
			}
			return result;
		}

		protected async Task<DataProviderInformation> MakeAndProcessHttpRequest(HttpClient httpClient, Uri requestUri, LinkPreviewDataProvider.ProcessResponseStreamDelegate processResponseStream)
		{
			DataProviderInformation dataProviderInformation;
			using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
			{
				if (!httpResponseMessage.IsSuccessStatusCode)
				{
					GetLinkPreview.ThrowInvalidRequestException(httpResponseMessage);
				}
				this.RequireContentType(httpResponseMessage.Content.Headers.ContentType);
				this.RestrictContentLength(httpResponseMessage.Content.Headers.ContentLength);
				this.ContentLength = ((httpResponseMessage.Content.Headers.ContentLength != null) ? httpResponseMessage.Content.Headers.ContentLength.Value : 0L);
				Encoding responseHeaderEncoding = this.GetResponseHeaderEncoding(httpResponseMessage.Content.Headers.ContentType.CharSet);
				Uri responseUri = httpResponseMessage.RequestMessage.RequestUri;
				int maxByteCount = this.GetMaxByteCount(responseUri);
				using (MemoryStream memoryStream = new MemoryStream(maxByteCount))
				{
					using (Stream responseStream = await httpResponseMessage.Content.ReadAsStreamAsync())
					{
						byte[] buffer = new byte[1024];
						int readCount = 0;
						int totalReadCount = 0;
						do
						{
							readCount = await responseStream.ReadAsync(buffer, 0, buffer.Length);
							memoryStream.Write(buffer, 0, readCount);
							totalReadCount += readCount;
						}
						while (readCount > 0 && totalReadCount < maxByteCount);
						if (totalReadCount <= 0)
						{
							GetLinkPreview.ThrowInvalidRequestException("EmptyContent", "Url returns no content.");
						}
					}
					dataProviderInformation = processResponseStream(responseUri, responseHeaderEncoding, memoryStream, this.logger);
				}
			}
			return dataProviderInformation;
		}

		private Encoding GetResponseHeaderEncoding(string characterSet)
		{
			Encoding result = null;
			if (characterSet != null)
			{
				try
				{
					return Encoding.GetEncoding(characterSet);
				}
				catch (ArgumentException)
				{
					GetLinkPreview.ThrowInvalidRequestException("GetEncodingFailed", string.Format("Get encoding failed for {0}", characterSet));
					return result;
				}
			}
			result = Encoding.GetEncoding("ISO-8859-1");
			return result;
		}

		private static Uri CreateUri(string url)
		{
			Uri uri = null;
			if (Uri.TryCreate(url, UriKind.Absolute, out uri))
			{
				string text = null;
				try
				{
					IdnMapping idnMapping = new IdnMapping();
					text = idnMapping.GetAscii(uri.Host);
				}
				catch (ArgumentException)
				{
					GetLinkPreview.ThrowInvalidRequestException("InvalidUrl", "Request url is invalid");
				}
				if (string.CompareOrdinal(text, uri.Host) != 0)
				{
					uri = new UriBuilder(uri)
					{
						Host = text
					}.Uri;
				}
				return uri;
			}
			GetLinkPreview.ThrowInvalidRequestException("InvalidUrl", "Request url is invalid");
			return uri;
		}

		private const string DefaultCharSet = "ISO-8859-1";

		protected const string HtmlContentType = "text/html";

		protected const string XHtmlContentType = "application/xhtml+xml";

		protected const string XmlContentType = "text/xml";

		protected const int MaxContentLength = 524288;

		protected const int MaxByteCount = 32768;

		protected const int RequestTimeoutInterval = 6000;

		protected Uri uri;

		protected readonly RequestDetailsLogger logger;

		protected readonly GetLinkPreviewRequest request;

		protected delegate DataProviderInformation ProcessResponseStreamDelegate(Uri responseUri, Encoding responseHeaderEncoding, MemoryStream memoryStream, RequestDetailsLogger logger);
	}
}
