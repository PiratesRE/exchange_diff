using System;
using System.Globalization;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal struct MailboxDeliveryGroupId : IEquatable<MailboxDeliveryGroupId>
	{
		public MailboxDeliveryGroupId(string stringId)
		{
			RoutingUtils.ThrowIfNullOrEmpty(stringId, "stringId");
			this.stringId = stringId;
		}

		public MailboxDeliveryGroupId(string siteName, int version)
		{
			RoutingUtils.ThrowIfNullOrEmpty(siteName, "siteName");
			this.stringId = string.Format(CultureInfo.InvariantCulture, "site:{0}; version:{1}", new object[]
			{
				siteName.ToLowerInvariant(),
				version
			});
		}

		public override int GetHashCode()
		{
			return this.stringId.GetHashCode();
		}

		public bool Equals(MailboxDeliveryGroupId other)
		{
			return this.stringId.Equals(other.stringId, StringComparison.Ordinal);
		}

		public override bool Equals(object other)
		{
			return other != null && other is MailboxDeliveryGroupId && this.Equals((MailboxDeliveryGroupId)other);
		}

		public override string ToString()
		{
			return this.stringId;
		}

		private const string Format = "site:{0}; version:{1}";

		private string stringId;
	}
}
