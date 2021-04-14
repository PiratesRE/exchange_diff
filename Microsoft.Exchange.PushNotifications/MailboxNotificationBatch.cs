using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class MailboxNotificationBatch
	{
		[DataMember(Name = "notifications", IsRequired = true, EmitDefaultValue = false)]
		public List<MailboxNotification> Notifications { get; set; }

		public bool IsEmpty
		{
			get
			{
				return this.Notifications == null || this.Notifications.Count == 0;
			}
		}

		public int Count
		{
			get
			{
				if (this.Notifications != null)
				{
					return this.Notifications.Count;
				}
				return 0;
			}
		}

		public void Add(MailboxNotification notification)
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (this.Notifications == null)
			{
				this.Notifications = new List<MailboxNotification>();
			}
			this.Notifications.Add(notification);
		}

		public bool IsMonitoring()
		{
			return this.Notifications != null && this.Notifications.Count > 0 && this.Notifications[0].Payload != null && this.Notifications[0].Payload.IsMonitoring;
		}

		public string ToJson()
		{
			return JsonConverter.Serialize<MailboxNotificationBatch>(this, null);
		}

		public string ToFullString()
		{
			return string.Format("{{ \"notifications\": {0} }}", this.Notifications.ToNullableString(null));
		}

		public override string ToString()
		{
			return string.Format("MailboxNotificationBatch: Count={0};", this.Count);
		}
	}
}
