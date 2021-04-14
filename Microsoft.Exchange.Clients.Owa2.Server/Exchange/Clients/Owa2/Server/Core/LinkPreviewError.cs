using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class LinkPreviewError
	{
		public const string InvalidUrl = "InvalidUrl";

		public const string UnsupportedContentType = "UnsupportedContentType";

		public const string MaxContentLengthExceeded = "MaxContentLengthExceeded";

		public const string EmptyContent = "EmptyContent";

		public const string TitleAndDescriptionNotFound = "TitleAndDescriptionNotFound";

		public const string DescriptionAndImageNotFound = "DescriptionAndImageNotFound";

		public const string TitleAndImageNotFound = "TitleAndImageNotFound";

		public const string RegExTimeout = "RegExTimeout";

		public const string RequestTimeout = "RequestTimeout";

		public const string MaxConcurrentRequestExceeded = "MaxConcurrentRequestExceeded";

		public const string MaxImageUrlLengthExceeded = "MaxImageUrlLengthExceeded";

		public const string InvalidImageUrl = "InvalidImageUrl";

		public const string HtmlConversionFailed = "HtmlConversionFailed";

		public const string EncodingGetStringFailed = "EncodingGetStringFailed";

		public const string GetEncodingFailed = "GetEncodingFailed";

		public const string OEmbedQueryStringNotFound = "OEmbedQueryStringNotFound";

		public const string OEmbedResponseNull = "OEmbedResponseNull";

		public const string OEmbedResponseHtmlNull = "OEmbedResponseHtmlNull";

		public const string OEmbedResponseSerializationReadFailed = "OEmbedResponseSerializationReadFailed";

		public const string InvalidUrlMessage = "Request url is invalid";

		public const string UnsupportedContentTypeMessage = "Content type {0} is not supported.";

		public const string MaxContentLengthExceededMessage = "Content length {0} exceeds maximum size allowed.";

		public const string EmptyContentMessage = "Url returns no content.";

		public const string TitleAndDescriptionNotFoundMessage = "No title or description were found.";

		public const string DescriptionAndImageNotFoundMessage = "No description or image were found.";

		public const string TitleAndImageNotFoundMessage = "No title or image were found.";

		public const string RegExTimeoutMessage = "The regex timed out on property {0}.";

		public const string RequestTimeoutMessage = "The web page request timed out.";

		public const string MaxConcurrentRequestExceededMessage = "The maximum number of concurrent requests has been exceeded.";

		public const string MaxImageUrlLengthExceededMessage = "Image url length {0} exceeds the maximum length allowed.";

		public const string InvalidImageUrlMessage = "Image url {0} is invalid.";

		public const string EncodingGetStringFailedMessage = "Encoding {0} failed with {1}";

		public const string GetEncodingFailedMessage = "Get encoding failed for {0}";

		public const string OEmbedQueryStringNotFoundMessage = "Could not get OEmbed query string for url {0}";

		public const string OEmbedResponseNullMessage = "The OEmbedResponse was null for the webpage information for {0}";

		public const string OEmbedResponseHtmlNullMessage = "The OEmbedResponse HTML was null for the webpage information for {0}";

		public const string OEmbedResponseSerializationReadFailedMeesage = "Failed to read the OEmbed response object. Error {0}";
	}
}
