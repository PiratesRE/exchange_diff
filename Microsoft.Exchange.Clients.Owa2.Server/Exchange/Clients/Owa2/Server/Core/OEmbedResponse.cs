using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class OEmbedResponse
	{
		[DataMember(Name = "type")]
		public string Type { get; set; }

		[DataMember(Name = "version")]
		public string Version { get; set; }

		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "author_name")]
		public string AuthorName { get; set; }

		[DataMember(Name = "author_url")]
		public string AuthorUrl { get; set; }

		[DataMember(Name = "provider_name")]
		public string ProviderName { get; set; }

		[DataMember(Name = "provider_url")]
		public string ProviderUrl { get; set; }

		[DataMember(Name = "cache_age")]
		public string CacheAge { get; set; }

		[DataMember(Name = "thumbnail_url")]
		public string ThumbnailUrl { get; set; }

		[DataMember(Name = "thumbnail_width")]
		public string ThumbnailWidth { get; set; }

		[DataMember(Name = "thumbnail_height")]
		public string ThumbnailHeight { get; set; }

		[DataMember(Name = "url")]
		public string Url { get; set; }

		[DataMember(Name = "html")]
		public string Html { get; set; }

		[DataMember(Name = "width")]
		public int Width { get; set; }

		[DataMember(Name = "height")]
		public int Height { get; set; }
	}
}
