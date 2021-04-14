using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class OutlookNotificationBatch
	{
		[DataMember(Name = "notifications", IsRequired = true, EmitDefaultValue = false)]
		public List<OutlookNotification> Notifications { get; set; }

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

		public void Add(OutlookNotification notification)
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (this.Notifications == null)
			{
				this.Notifications = new List<OutlookNotification>();
			}
			this.Notifications.Add(notification);
		}

		public string ToJson()
		{
			return JsonConverter.Serialize<OutlookNotificationBatch>(this, null);
		}

		public string ToFullString()
		{
			return string.Format("{{ \"notifications\": {0} }}", this.Notifications.ToNullableString(null));
		}

		public override string ToString()
		{
			return string.Format("OutlookNotificationBatch: Count={0};", this.Count);
		}
	}
}
