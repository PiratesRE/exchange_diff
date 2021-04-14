using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class AggregationSubscriptionIdParameter : IIdentityParameter
	{
		public AggregationSubscriptionIdParameter()
		{
		}

		public AggregationSubscriptionIdParameter(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (id.Length == 0)
			{
				throw new ArgumentException(Strings.ErrorEmptyParameter(base.GetType().ToString()), "identity");
			}
			this.Parse(id);
		}

		public AggregationSubscriptionIdParameter(ObjectId subId)
		{
			this.Initialize(subId);
		}

		public AggregationSubscriptionIdParameter(Mailbox mailbox)
		{
			this.mailboxId = new MailboxIdParameter(mailbox);
		}

		public AggregationSubscriptionIdParameter(PimSubscriptionProxy subscriptionProxy) : this(subscriptionProxy.Identity)
		{
		}

		public AggregationSubscriptionIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.RawIdentity;
			}
		}

		public string RawIdentity
		{
			get
			{
				return this.ToString();
			}
		}

		public string Name
		{
			get
			{
				return this.subscriptionName;
			}
			set
			{
				this.subscriptionName = value;
			}
		}

		public Guid SubscriptionId
		{
			get
			{
				return this.subscriptionId;
			}
			set
			{
				this.subscriptionId = value;
			}
		}

		public MailboxIdParameter MailboxIdParameter
		{
			get
			{
				return this.mailboxId;
			}
		}

		public SmtpAddress EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
			set
			{
				this.emailAddress = value;
			}
		}

		public string GuidIdentityString
		{
			get
			{
				if (this.subscriptionIdentity != null)
				{
					return this.subscriptionIdentity.GuidIdentityString;
				}
				throw new InvalidOperationException("SubscriptionIdentity should not be null.");
			}
		}

		internal AggregationSubscriptionType? SubscriptionType
		{
			get
			{
				return this.subscriptionType;
			}
			set
			{
				this.subscriptionType = value;
			}
		}

		internal AggregationType? AggregationType
		{
			get
			{
				return this.aggregationType;
			}
			set
			{
				this.aggregationType = value;
			}
		}

		internal AggregationSubscriptionIdentity SubscriptionIdentity
		{
			get
			{
				return this.subscriptionIdentity;
			}
		}

		internal bool IsUniqueIdentity
		{
			get
			{
				return this.SubscriptionIdentity != null || this.Name != null || this.EmailAddress != SmtpAddress.Empty || this.SubscriptionId != Guid.Empty;
			}
		}

		public void Initialize(ObjectId subId)
		{
			AggregationSubscriptionIdentity aggregationSubscriptionIdentity = subId as AggregationSubscriptionIdentity;
			if (aggregationSubscriptionIdentity != null)
			{
				this.subscriptionIdentity = aggregationSubscriptionIdentity;
				this.mailboxId = new MailboxIdParameter(aggregationSubscriptionIdentity.AdUserId);
			}
		}

		public override string ToString()
		{
			if (this.subscriptionIdentity != null)
			{
				return this.subscriptionIdentity.ToString();
			}
			if (this.subscriptionName != null)
			{
				return this.subscriptionName;
			}
			if (this.emailAddress != SmtpAddress.Empty)
			{
				return this.emailAddress.ToString();
			}
			if (this.subscriptionId != Guid.Empty)
			{
				return this.subscriptionId.ToString();
			}
			return string.Empty;
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return ((IIdentityParameter)this).GetObjects<T>(rootId, session, null, out localizedString);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = null;
			AggregationSubscriptionQueryFilter filter = new AggregationSubscriptionQueryFilter(this);
			return session.FindPaged<T>(filter, rootId, false, null, 0);
		}

		private void Parse(string identity)
		{
			identity = identity.Trim();
			if (string.IsNullOrEmpty(identity))
			{
				return;
			}
			if (this.TryParseDNAndGuid(identity))
			{
				return;
			}
			try
			{
				this.emailAddress = SmtpAddress.Parse(identity);
			}
			catch (FormatException)
			{
			}
			if (this.emailAddress == SmtpAddress.Empty && !GuidHelper.TryParseGuid(identity, out this.subscriptionId))
			{
				this.subscriptionName = identity;
			}
		}

		private bool TryParseDNAndGuid(string input)
		{
			if (input == null || input.Length < 3)
			{
				return false;
			}
			int num = input.LastIndexOf('/');
			if (num == -1)
			{
				return false;
			}
			this.mailboxId = new MailboxIdParameter(input.Substring(0, num));
			if (!GuidHelper.TryParseGuid(input.Substring(num + 1), out this.subscriptionId))
			{
				return false;
			}
			this.subscriptionIdentity = new AggregationSubscriptionIdentity(this.mailboxId.InternalADObjectId, this.subscriptionId);
			return true;
		}

		private AggregationSubscriptionIdentity subscriptionIdentity;

		private MailboxIdParameter mailboxId;

		private string subscriptionName;

		private Guid subscriptionId = Guid.Empty;

		private SmtpAddress emailAddress = SmtpAddress.Empty;

		private AggregationSubscriptionType? subscriptionType;

		private AggregationType? aggregationType;
	}
}
