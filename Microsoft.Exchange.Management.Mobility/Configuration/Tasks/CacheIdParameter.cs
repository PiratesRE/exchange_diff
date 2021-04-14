using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class CacheIdParameter : IIdentityParameter
	{
		public CacheIdParameter()
		{
		}

		public CacheIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
		}

		public CacheIdParameter(string identity)
		{
			this.mailboxId = new MailboxIdParameter(identity);
		}

		public CacheIdParameter(PimSubscriptionProxy subscriptionProxy) : this(subscriptionProxy.Subscription.AdUserId)
		{
		}

		public CacheIdParameter(SubscriptionsCache cache)
		{
			this.Initialize(cache.Identity);
		}

		public CacheIdParameter(AggregationSubscriptionIdentity subscriptionIdentity) : this(subscriptionIdentity.AdUserId)
		{
		}

		public CacheIdParameter(AggregationSubscriptionIdParameter subscriptionIdParameter) : this(subscriptionIdParameter.MailboxIdParameter)
		{
		}

		public CacheIdParameter(MailboxIdParameter mailboxIdParameter)
		{
			this.mailboxId = mailboxIdParameter;
		}

		public CacheIdParameter(Mailbox mailbox) : this(mailbox.Id)
		{
		}

		public CacheIdParameter(ADObjectId objectId)
		{
			this.Initialize(objectId);
		}

		public string RawIdentity
		{
			get
			{
				return this.mailboxId.ToString();
			}
		}

		internal MailboxIdParameter MailboxId
		{
			get
			{
				return this.mailboxId;
			}
		}

		public override string ToString()
		{
			return this.mailboxId.ToString();
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = null;
			return session.FindPaged<T>(null, rootId, false, null, 0);
		}

		public void Initialize(ObjectId objectId)
		{
			this.mailboxId = new MailboxIdParameter((ADObjectId)objectId);
		}

		private MailboxIdParameter mailboxId;
	}
}
