using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ImapSubscription : PimSubscription
	{
		public ImapSubscription(IMAPSubscriptionProxy subscription) : base(subscription)
		{
			this.ImapSubscriptionProxy = subscription;
		}

		public IMAPSubscriptionProxy ImapSubscriptionProxy { get; private set; }

		[DataMember]
		public string IncomingUserName
		{
			get
			{
				return this.ImapSubscriptionProxy.IncomingUserName;
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
				if (this.ImapSubscriptionProxy.IncomingServer == null)
				{
					return null;
				}
				return this.ImapSubscriptionProxy.IncomingServer.Domain;
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
				return this.ImapSubscriptionProxy.IncomingPort;
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
				return this.ImapSubscriptionProxy.IncomingSecurity.ToString();
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
				return this.ImapSubscriptionProxy.IncomingAuthentication.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
