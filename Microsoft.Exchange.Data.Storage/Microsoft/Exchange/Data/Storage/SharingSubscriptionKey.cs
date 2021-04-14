using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct SharingSubscriptionKey : IEquatable<SharingSubscriptionKey>
	{
		public SharingSubscriptionKey(string sharerIdentity, string remoteFolderId)
		{
			this.sharerIdentity = sharerIdentity;
			this.remoteFolderId = remoteFolderId;
		}

		public string SharerIdentity
		{
			get
			{
				return this.sharerIdentity;
			}
		}

		public string RemoteFolderId
		{
			get
			{
				return this.remoteFolderId;
			}
		}

		public bool Equals(SharingSubscriptionKey other)
		{
			return StringComparer.InvariantCultureIgnoreCase.Equals(this.SharerIdentity, other.SharerIdentity) && StringComparer.InvariantCulture.Equals(this.RemoteFolderId, other.RemoteFolderId);
		}

		public override string ToString()
		{
			return this.SharerIdentity + ":" + this.RemoteFolderId;
		}

		private string sharerIdentity;

		private string remoteFolderId;
	}
}
