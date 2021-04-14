using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[Serializable]
	public class AggregationSubscriptionIdentity : ObjectId, IEquatable<AggregationSubscriptionIdentity>
	{
		public AggregationSubscriptionIdentity()
		{
			this.subscriptionId = Guid.Empty;
		}

		public AggregationSubscriptionIdentity(string id)
		{
			SyncUtilities.ThrowIfArgumentNull("id", id);
			int num = id.LastIndexOf('/');
			if (num <= 0 || num == id.Length - 1)
			{
				throw new ArgumentException(Strings.InvalidAggregationSubscriptionIdentity, "id");
			}
			string input = id.Substring(0, num);
			string g = id.Substring(num + 1);
			this.userId = ADObjectId.ParseDnOrGuid(input);
			this.subscriptionId = new Guid(g);
		}

		public AggregationSubscriptionIdentity(ADObjectId userId, Guid subscriptionId)
		{
			SyncUtilities.ThrowIfArgumentNull("userId", userId);
			SyncUtilities.ThrowIfGuidEmpty("subscriptionId", subscriptionId);
			this.userId = userId;
			this.subscriptionId = subscriptionId;
		}

		public AggregationSubscriptionIdentity(Guid subscriptionId, string legacyDN, string primaryMailboxLegacyDN)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionId", subscriptionId);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("legacyDN", legacyDN);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("primaryMailboxLegacyDN", primaryMailboxLegacyDN);
			this.subscriptionId = subscriptionId;
			this.legacyDN = legacyDN;
			this.primaryMailboxLegacyDN = primaryMailboxLegacyDN;
		}

		public ADObjectId AdUserId
		{
			get
			{
				return this.userId;
			}
			internal set
			{
				this.userId = value;
			}
		}

		public Guid SubscriptionId
		{
			get
			{
				return this.subscriptionId;
			}
			internal set
			{
				this.subscriptionId = value;
			}
		}

		public string LegacyDN
		{
			get
			{
				return this.legacyDN;
			}
			internal set
			{
				this.legacyDN = value;
			}
		}

		public string PrimaryMailboxLegacyDN
		{
			get
			{
				return this.primaryMailboxLegacyDN;
			}
			internal set
			{
				this.primaryMailboxLegacyDN = value;
			}
		}

		public string GuidIdentityString
		{
			get
			{
				if (this.userId == null)
				{
					throw new InvalidOperationException("adUserId should not be null.");
				}
				if (this.subscriptionId == Guid.Empty)
				{
					throw new InvalidOperationException("subscriptionId should not be an Empty Guid.");
				}
				return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[]
				{
					this.userId.ObjectGuid.ToString(),
					'/',
					this.subscriptionId
				});
			}
		}

		public override byte[] GetBytes()
		{
			return null;
		}

		public override string ToString()
		{
			if (this.userId == null || this.subscriptionId == Guid.Empty)
			{
				return string.Empty;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[]
			{
				this.userId.ToDNString(),
				'/',
				this.subscriptionId
			});
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as AggregationSubscriptionIdentity);
		}

		public bool Equals(AggregationSubscriptionIdentity other)
		{
			return other != null && this.userId != null && this.userId.Equals(other.userId) && this.subscriptionId == other.SubscriptionId;
		}

		public override int GetHashCode()
		{
			if (this.userId == null)
			{
				return 0;
			}
			return this.userId.GetHashCode() | this.subscriptionId.GetHashCode();
		}

		private const char Separator = '/';

		private ADObjectId userId;

		private Guid subscriptionId;

		private string legacyDN;

		private string primaryMailboxLegacyDN;
	}
}
