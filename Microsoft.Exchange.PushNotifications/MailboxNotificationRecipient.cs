using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class MailboxNotificationRecipient : BasicNotificationRecipient
	{
		public MailboxNotificationRecipient(string appId, string deviceId, DateTime lastSubscriptionUpdate, string installationId = null) : base(appId, deviceId)
		{
			this.LastSubscriptionUpdate = lastSubscriptionUpdate;
			this.InstallationId = installationId;
		}

		[DataMember(Name = "lastSubscriptionUpdate", EmitDefaultValue = false)]
		public DateTime LastSubscriptionUpdate { get; private set; }

		[DataMember(Name = "InstallationId", EmitDefaultValue = false)]
		public string InstallationId { get; private set; }

		internal static MailboxNotificationRecipient CreateMonitoringRecipient(string appId, int deviceId, PushNotificationPlatform platform)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("appId", appId);
			ArgumentValidator.ThrowIfNegative("deviceId", deviceId);
			switch (platform)
			{
			case PushNotificationPlatform.APNS:
				return new MailboxNotificationRecipient(appId, string.Format("{0:d64}", deviceId), DateTime.UtcNow, null);
			case PushNotificationPlatform.WNS:
				return new MailboxNotificationRecipient(appId, string.Format("http://127.0.0.1:0/send?id={0}", deviceId), DateTime.UtcNow, null);
			case PushNotificationPlatform.GCM:
			case PushNotificationPlatform.WebApp:
				return new MailboxNotificationRecipient(appId, deviceId.ToString(), DateTime.UtcNow, null);
			}
			throw new InvalidOperationException(string.Format("Platform {0} is not supported for monitoring", platform.ToString()));
		}

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("lastSubscriptionUpdate:").Append(this.LastSubscriptionUpdate.ToString()).Append("; ");
			sb.Append("installationId:").Append(this.InstallationId ?? "null");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			if ((ExDateTime)this.LastSubscriptionUpdate > ExDateTime.UtcNow)
			{
				errors.Add(Strings.InvalidMnRecipientLastSubscriptionUpdate(this.LastSubscriptionUpdate.ToString()));
			}
		}
	}
}
