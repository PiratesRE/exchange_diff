using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class PopSubscription : PimSubscription
	{
		public PopSubscription(PopSubscriptionProxy subscription) : base(subscription)
		{
			this.PopSubscriptionProxy = subscription;
		}

		public PopSubscriptionProxy PopSubscriptionProxy { get; private set; }

		[DataMember]
		public string IncomingUserName
		{
			get
			{
				return this.PopSubscriptionProxy.IncomingUserName;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string IncomingServer
		{
			get
			{
				if (this.PopSubscriptionProxy.IncomingServer == null)
				{
					return null;
				}
				return this.PopSubscriptionProxy.IncomingServer.Domain;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int IncomingPort
		{
			get
			{
				return this.PopSubscriptionProxy.IncomingPort;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string IncomingSecurity
		{
			get
			{
				return this.PopSubscriptionProxy.IncomingSecurity.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string IncomingAuth
		{
			get
			{
				return this.PopSubscriptionProxy.IncomingAuthentication.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool LeaveOnServer
		{
			get
			{
				return this.PopSubscriptionProxy.LeaveOnServer;
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
