using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[Serializable]
	public class PushNotificationStoreId : XsoMailboxObjectId
	{
		internal PushNotificationStoreId(ADObjectId mailboxOwnerId, StoreObjectId storeObjectIdValue, string subscriptionId) : base(mailboxOwnerId)
		{
			ArgumentValidator.ThrowIfNull("storeObjectId", storeObjectIdValue);
			this.StoreObjectId = storeObjectIdValue.ToString();
			this.StoreObjectIdValue = storeObjectIdValue;
			this.SubscriptionId = subscriptionId;
		}

		internal PushNotificationStoreId(ADObjectId mailboxOwnerId, StoreObjectId storeObjectIdValue) : this(mailboxOwnerId, storeObjectIdValue, string.Empty)
		{
		}

		public string SubscriptionId { get; private set; }

		public string StoreObjectId { get; set; }

		internal StoreObjectId StoreObjectIdValue { get; private set; }

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.SubscriptionId))
			{
				return string.Format("{0}\\{1}", base.MailboxOwnerId.Name, this.StoreObjectId);
			}
			return string.Format("{0}\\{1} ({2})", base.MailboxOwnerId.Name, this.StoreObjectId, this.SubscriptionId);
		}
	}
}
