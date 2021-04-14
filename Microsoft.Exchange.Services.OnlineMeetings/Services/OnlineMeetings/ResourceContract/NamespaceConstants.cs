using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	internal static class NamespaceConstants
	{
		internal static class ContainerNames
		{
			public const string EventsRequest = "EventsRequest";

			public const string EventsResponse = "EventsResponse";

			public const string MultipartRelatedEventsResponse = "MultipartRelatedEventsResponse";

			public const string MultipartRelatedRequest = "MultipartRelatedRequest";
		}

		internal static class MediaTypes
		{
			internal static class Hal
			{
				public const string Json = "application/hal+json";

				public const string Xml = "application/hal+xml";
			}

			internal static class Xml
			{
				public const string Common = "application/vnd.microsoft.com.ucwa+xml";

				public const string Error = "application/vnd.microsoft.com.ucwa.error+xml";

				public const string Request = "application/vnd.microsoft.com.ucwa.request+xml";

				public const string Events = "application/vnd.microsoft.com.ucwa.events+xml";
			}

			internal static class Json
			{
				public const string Common = "application/vnd.microsoft.com.ucwa+json";

				public const string Error = "application/vnd.microsoft.com.ucwa.error+json";

				public const string Request = "application/vnd.microsoft.com.ucwa.request+json";

				public const string Events = "application/vnd.microsoft.com.ucwa.events+json";
			}

			internal static class Text
			{
				public const string Plain = "text/plain";

				public const string Html = "text/html";
			}

			internal static class Mime
			{
				public const string Related = "multipart/related";

				public const string Alternative = "multipart/alternative";

				public const string BatchingSubtype = "batching";
			}

			internal static class MimeXml
			{
				public const string Related = "multipart/related+xml";
			}

			internal static class MimeJson
			{
				public const string Related = "multipart/related+json";
			}

			internal static class Application
			{
				public const string Sdp = "application/sdp";

				public const string Json = "application/json";

				public const string Xml = "application/xml";
			}

			internal static class Photo
			{
				public const string Jpeg = "image/jpeg";
			}
		}
	}
}
