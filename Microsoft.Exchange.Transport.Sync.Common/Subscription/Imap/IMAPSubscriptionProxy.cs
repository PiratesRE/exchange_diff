using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap
{
	[Serializable]
	public sealed class IMAPSubscriptionProxy : PimSubscriptionProxy
	{
		public IMAPSubscriptionProxy() : this(new IMAPAggregationSubscription())
		{
		}

		internal IMAPSubscriptionProxy(IMAPAggregationSubscription subscription) : base(subscription)
		{
		}

		public Fqdn IncomingServer
		{
			get
			{
				return ((IMAPAggregationSubscription)base.Subscription).IMAPServer;
			}
			set
			{
				((IMAPAggregationSubscription)base.Subscription).IMAPServer = value;
			}
		}

		public int IncomingPort
		{
			get
			{
				return ((IMAPAggregationSubscription)base.Subscription).IMAPPort;
			}
			set
			{
				((IMAPAggregationSubscription)base.Subscription).IMAPPort = value;
			}
		}

		public string IncomingUserName
		{
			get
			{
				return base.RedactIfNeeded(((IMAPAggregationSubscription)base.Subscription).IMAPLogOnName, false);
			}
			set
			{
				((IMAPAggregationSubscription)base.Subscription).IMAPLogOnName = value;
			}
		}

		public IMAPSecurityMechanism IncomingSecurity
		{
			get
			{
				return ((IMAPAggregationSubscription)base.Subscription).IMAPSecurity;
			}
			set
			{
				((IMAPAggregationSubscription)base.Subscription).IMAPSecurity = value;
			}
		}

		public IMAPAuthenticationMechanism IncomingAuthentication
		{
			get
			{
				return ((IMAPAggregationSubscription)base.Subscription).IMAPAuthentication;
			}
			set
			{
				((IMAPAggregationSubscription)base.Subscription).IMAPAuthentication = value;
			}
		}

		public static IMAPSubscriptionProxy ShallowCopy(IMAPSubscriptionProxy incoming)
		{
			return new IMAPSubscriptionProxy
			{
				IncomingServer = incoming.IncomingServer,
				IncomingPort = incoming.IncomingPort,
				IncomingUserName = incoming.IncomingUserName,
				IncomingSecurity = incoming.IncomingSecurity,
				IncomingAuthentication = incoming.IncomingAuthentication,
				Name = incoming.Name,
				DisplayName = incoming.DisplayName,
				EmailAddress = incoming.EmailAddress
			};
		}

		public override ValidationError[] Validate()
		{
			ICollection<ValidationError> collection = IMAPSubscriptionValidator.Validate(this);
			ValidationError[] array = new ValidationError[collection.Count];
			collection.CopyTo(array, 0);
			return array;
		}

		public override void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		public void SetPassword(SecureString password)
		{
			base.Subscription.LogonPasswordSecured = password;
		}
	}
}
