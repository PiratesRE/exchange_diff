using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "Link")]
	internal class Link
	{
		public Link()
		{
		}

		public Link(string token, string href) : this(token, href, "related")
		{
		}

		public Link(string token, string href, string rel)
		{
			if (string.IsNullOrEmpty(token))
			{
				throw new ArgumentException("token");
			}
			if (string.IsNullOrEmpty(href))
			{
				throw new ArgumentException("href");
			}
			this.Relationship = rel;
			this.Href = href;
			this.Token = token;
		}

		public Link(Link copy)
		{
			if (copy == null)
			{
				throw new ArgumentNullException("copy");
			}
			this.Cid = copy.Cid;
			this.Href = copy.Href;
			this.Relationship = copy.Relationship;
			this.Token = copy.Token;
		}

		[IgnoreDataMember]
		public string Token { get; set; }

		[DataMember(Name = "cid", EmitDefaultValue = false)]
		public string Cid { get; set; }

		[DataMember(Name = "rel", EmitDefaultValue = false)]
		public string Relationship { get; set; }

		[DataMember(Name = "href", EmitDefaultValue = false)]
		public string Href { get; set; }

		[IgnoreDataMember]
		public object Target { get; set; }

		public bool CanBeEmbedded
		{
			get
			{
				Resource resource = this.Target as Resource;
				return resource != null && resource.CanBeEmbedded;
			}
		}

		internal static class Relationships
		{
			public const string Added = "added";

			public const string Creator = "creator";

			public const string Deleted = "deleted";

			public const string Completed = "completed";

			public const string Next = "next";

			public const string Public = "public";

			public const string Related = "related";

			public const string Self = "self";

			public const string Sender = "sender";

			public const string Updated = "updated";
		}
	}
}
