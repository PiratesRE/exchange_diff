using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop
{
	[Serializable]
	public sealed class PopSubscriptionProxy : PimSubscriptionProxy
	{
		public PopSubscriptionProxy() : this(new PopAggregationSubscription())
		{
		}

		internal PopSubscriptionProxy(PopAggregationSubscription subscription) : base(subscription)
		{
		}

		public Fqdn IncomingServer
		{
			get
			{
				return ((PopAggregationSubscription)base.Subscription).PopServer;
			}
			set
			{
				((PopAggregationSubscription)base.Subscription).PopServer = value;
			}
		}

		public int IncomingPort
		{
			get
			{
				return ((PopAggregationSubscription)base.Subscription).PopPort;
			}
			set
			{
				((PopAggregationSubscription)base.Subscription).PopPort = value;
			}
		}

		public string IncomingUserName
		{
			get
			{
				return base.RedactIfNeeded(((PopAggregationSubscription)base.Subscription).PopLogonName, false);
			}
			set
			{
				((PopAggregationSubscription)base.Subscription).PopLogonName = value;
			}
		}

		public SecurityMechanism IncomingSecurity
		{
			get
			{
				return ((PopAggregationSubscription)base.Subscription).PopSecurity;
			}
			set
			{
				((PopAggregationSubscription)base.Subscription).PopSecurity = value;
			}
		}

		public AuthenticationMechanism IncomingAuthentication
		{
			get
			{
				return ((PopAggregationSubscription)base.Subscription).PopAuthentication;
			}
			set
			{
				((PopAggregationSubscription)base.Subscription).PopAuthentication = value;
			}
		}

		public bool LeaveOnServer
		{
			get
			{
				return ((PopAggregationSubscription)base.Subscription).ShouldLeaveOnServer;
			}
			set
			{
				((PopAggregationSubscription)base.Subscription).ShouldLeaveOnServer = value;
			}
		}

		public bool ServerSupportsMirroring
		{
			get
			{
				return this.serverSupportsMirroring;
			}
			set
			{
				this.serverSupportsMirroring = value;
			}
		}

		public override ValidationError[] Validate()
		{
			ICollection<ValidationError> collection = PopSubscriptionValidator.Validate(this);
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

		private bool serverSupportsMirroring = true;
	}
}
