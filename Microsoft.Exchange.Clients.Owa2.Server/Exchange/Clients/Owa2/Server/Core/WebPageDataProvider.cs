using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class WebPageDataProvider : LinkPreviewDataProvider
	{
		public WebPageDataProvider(Uri uri, GetLinkPreviewRequest request, RequestDetailsLogger logger) : base(uri, request, logger)
		{
		}

		public override GetLinkPreviewResponse CreatePreview(DataProviderInformation dataProviderInformation)
		{
			string text = ((WebPageInformation)dataProviderInformation).Text;
			Dictionary<string, string> queryParmDictionary;
			LinkPreviewBuilder linkPreviewBuilder;
			if (YouTubeLinkPreviewBuilder.TryGetYouTubePlayerQueryParms(dataProviderInformation.ResponseUri, this.logger, out queryParmDictionary))
			{
				linkPreviewBuilder = new YouTubeLinkPreviewBuilder(queryParmDictionary, this.request, text, this.logger, dataProviderInformation.ResponseUri);
			}
			else if (AmazonLinkPreviewBuilder.IsAmazonUri(dataProviderInformation.ResponseUri))
			{
				linkPreviewBuilder = new AmazonLinkPreviewBuilder(this.request, text, this.logger, dataProviderInformation.ResponseUri);
			}
			else if (CraigsListLinkPreviewBuilder.IsCraigsListUri(dataProviderInformation.ResponseUri))
			{
				linkPreviewBuilder = new CraigsListLinkPreviewBuilder(this.request, text, this.logger, dataProviderInformation.ResponseUri);
			}
			else
			{
				linkPreviewBuilder = new WebPageLinkPreviewBuilder(this.request, text, this.logger, dataProviderInformation.ResponseUri, false);
			}
			return linkPreviewBuilder.Execute();
		}

		protected override async Task<DataProviderInformation> GetDataProviderInformation(HttpClient httpClient)
		{
			return await base.MakeAndProcessHttpRequest(httpClient, this.uri, new LinkPreviewDataProvider.ProcessResponseStreamDelegate(WebPageDataProvider.ProcessResponseStream));
		}

		public static DataProviderInformation ProcessResponseStream(Uri responseUri, Encoding responseHeaderEncoding, MemoryStream memoryStream, RequestDetailsLogger logger)
		{
			byte[] buffer = memoryStream.GetBuffer();
			string @string = WebPageDataProvider.GetString(responseHeaderEncoding, buffer);
			Encoding webPageEncoding = WebPageDataProvider.GetWebPageEncoding(@string, logger);
			if (webPageEncoding != null && !responseHeaderEncoding.Equals(webPageEncoding))
			{
				@string = WebPageDataProvider.GetString(webPageEncoding, buffer);
				logger.Set(GetLinkPreviewMetadata.WebPageEncodingUsed, 1);
			}
			return new WebPageInformation
			{
				Text = @string,
				ResponseUri = responseUri
			};
		}

		protected override int GetMaxByteCount(Uri responseUri)
		{
			if (AmazonLinkPreviewBuilder.IsAmazonUri(responseUri))
			{
				return 491520;
			}
			if (responseUri.Host != null && responseUri.Host.ToUpper().Equals("WWW.GROUPON.COM", StringComparison.Ordinal))
			{
				return 98304;
			}
			return 32768;
		}

		private static string GetString(Encoding encoding, byte[] bytes)
		{
			string result = null;
			try
			{
				result = encoding.GetString(bytes);
			}
			catch (DecoderFallbackException ex)
			{
				GetLinkPreview.ThrowInvalidRequestException("EncodingGetStringFailed", string.Format("Encoding {0} failed with {1}", encoding.EncodingName, ex.Message));
			}
			catch (ArgumentException ex2)
			{
				GetLinkPreview.ThrowInvalidRequestException("EncodingGetStringFailed", string.Format("Encoding {0} failed with {1}", encoding.EncodingName, ex2.Message));
			}
			return result;
		}

		private static Encoding GetWebPageEncoding(string webPageString, RequestDetailsLogger logger)
		{
			Encoding result = null;
			string text = null;
			int num = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			text = WebPageDataProvider.GetWebPageEncoding(webPageString, WebPageDataProvider.GetXmlEncodingRegEx, "xml encoding");
			num++;
			if (text == null)
			{
				text = WebPageDataProvider.GetWebPageEncoding(webPageString, WebPageDataProvider.GetMetaEncodingRegEx, "meta encoding");
				num++;
			}
			if (text == null)
			{
				text = WebPageDataProvider.GetWebPageEncoding(webPageString, WebPageDataProvider.GetMeta5EncodingRegEx, "meta encoding");
				num++;
			}
			if (text != null)
			{
				try
				{
					result = Encoding.GetEncoding(text);
				}
				catch (ArgumentException)
				{
					GetLinkPreview.ThrowInvalidRequestException("GetEncodingFailed", string.Format("Get encoding failed for {0}", text));
				}
			}
			stopwatch.Stop();
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			logger.Set(GetLinkPreviewMetadata.EncodingRegExCount, num);
			logger.Set(GetLinkPreviewMetadata.ElapsedTimeToGetWebPageEncoding, elapsedMilliseconds);
			return result;
		}

		private static string GetWebPageEncoding(string webPageString, Regex regEx, string propertyName)
		{
			string result = null;
			Match match = LinkPreviewBuilder.ExecuteRegEx(regEx, webPageString, propertyName);
			if (match.Groups["encoding"].Captures.Count > 0)
			{
				result = match.Groups["encoding"].Value;
			}
			return result;
		}

		protected override void RequireContentType(MediaTypeHeaderValue contentType)
		{
			if (contentType == null || string.IsNullOrWhiteSpace(contentType.MediaType))
			{
				GetLinkPreview.ThrowInvalidRequestException("UnsupportedContentType", string.Format("Content type {0} is not supported.", "null"));
			}
			string text = contentType.MediaType.ToLower();
			if (!text.Contains("text/html") && !text.Contains("application/xhtml+xml"))
			{
				GetLinkPreview.ThrowInvalidRequestException("UnsupportedContentType", string.Format("Content type {0} is not supported.", text));
			}
		}

		protected override void RestrictContentLength(long? contentLength)
		{
			if (contentLength != null && contentLength.Value > 524288L)
			{
				GetLinkPreview.ThrowInvalidRequestException("MaxContentLengthExceeded", string.Format("Content length {0} exceeds maximum size allowed.", contentLength.Value));
			}
		}

		private const int AmazonMaxByteCount = 491520;

		private const int GrouponMaxByteCount = 98304;

		private const string GrouponHostUpperCase = "WWW.GROUPON.COM";

		private const string EncodingRegExKey = "encoding";

		private const string XmlEncodingRegEx = "<\\?xml [^><]*?encoding=('|\")(?<encoding>.*?)('|\")[^><]*?>";

		private const string MetaEncodingRegEx = "<meta [^><]*?content=('|\")[^><]*?charset=(?<encoding>.*?)('|\")[^><]*?>";

		private const string Meta5EncodingRegEx = "<meta charset=('|\")(?<encoding>.*?)('|\")[^><]*?>";

		private const string XmlEncodingPropertyName = "xml encoding";

		private const string MetaEncodingPropertyName = "meta encoding";

		private const string Meta5EncodingPropertyName = "meta 5 encoding";

		private static Regex GetXmlEncodingRegEx = new Regex("<\\?xml [^><]*?encoding=('|\")(?<encoding>.*?)('|\")[^><]*?>", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);

		private static Regex GetMetaEncodingRegEx = new Regex("<meta [^><]*?content=('|\")[^><]*?charset=(?<encoding>.*?)('|\")[^><]*?>", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);

		private static Regex GetMeta5EncodingRegEx = new Regex("<meta charset=('|\")(?<encoding>.*?)('|\")[^><]*?>", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);
	}
}
