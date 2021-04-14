using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct PublishingSubscriptionKey : IEquatable<PublishingSubscriptionKey>
	{
		public PublishingSubscriptionKey(Uri publishingUrl)
		{
			this.publishingUrl = publishingUrl;
		}

		public Uri PublishingUrl
		{
			get
			{
				return this.publishingUrl;
			}
		}

		public bool Equals(PublishingSubscriptionKey other)
		{
			return object.Equals(this.PublishingUrl, other.PublishingUrl);
		}

		public override string ToString()
		{
			return this.PublishingUrl.OriginalString;
		}

		private Uri publishingUrl;
	}
}
