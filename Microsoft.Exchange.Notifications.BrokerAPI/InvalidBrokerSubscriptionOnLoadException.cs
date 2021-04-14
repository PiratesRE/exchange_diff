using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Notifications.Broker
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidBrokerSubscriptionOnLoadException : NotificationsBrokerPermanentException
	{
		public InvalidBrokerSubscriptionOnLoadException(string storeId, string mailbox) : base(ClientAPIStrings.InvalidBrokerSubscriptionOnLoadException(storeId, mailbox))
		{
			this.storeId = storeId;
			this.mailbox = mailbox;
		}

		public InvalidBrokerSubscriptionOnLoadException(string storeId, string mailbox, Exception innerException) : base(ClientAPIStrings.InvalidBrokerSubscriptionOnLoadException(storeId, mailbox), innerException)
		{
			this.storeId = storeId;
			this.mailbox = mailbox;
		}

		protected InvalidBrokerSubscriptionOnLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.storeId = (string)info.GetValue("storeId", typeof(string));
			this.mailbox = (string)info.GetValue("mailbox", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("storeId", this.storeId);
			info.AddValue("mailbox", this.mailbox);
		}

		public string StoreId
		{
			get
			{
				return this.storeId;
			}
		}

		public string Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		private readonly string storeId;

		private readonly string mailbox;
	}
}
