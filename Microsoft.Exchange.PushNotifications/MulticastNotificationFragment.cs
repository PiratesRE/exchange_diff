using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications
{
	internal class MulticastNotificationFragment<TPayload, TRecipient> : Notification where TPayload : BasicDataContract where TRecipient : BasicNotificationRecipient
	{
		public MulticastNotificationFragment(string notificationId, TPayload payload, TRecipient recipient) : this(new MulticastNotificationFragment<TPayload, TRecipient>.FragmentId<TRecipient>(notificationId, recipient), payload, recipient)
		{
		}

		private MulticastNotificationFragment(MulticastNotificationFragment<TPayload, TRecipient>.FragmentId<TRecipient> fragmentId, TPayload payload, TRecipient recipient) : base(fragmentId.AppId, fragmentId.RecipientId, fragmentId.Identifier)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("notificationId", fragmentId.NotificationId);
			this.NotificationIdentifier = fragmentId.NotificationId;
			this.Payload = payload;
			this.Recipient = recipient;
		}

		public string NotificationIdentifier { get; private set; }

		public TPayload Payload { get; private set; }

		public TRecipient Recipient { get; private set; }

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("notificationId:").Append(this.NotificationIdentifier).Append("; ");
			StringBuilder stringBuilder = sb.Append("payload:");
			TPayload payload = this.Payload;
			stringBuilder.Append(payload.ToFullString()).Append("; ");
			StringBuilder stringBuilder2 = sb.Append("recipients:");
			TRecipient recipient = this.Recipient;
			stringBuilder2.Append(recipient.ToFullString()).Append("; ");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			if (this.Payload == null)
			{
				errors.Add(Strings.InvalidPayload);
			}
			else
			{
				TPayload payload = this.Payload;
				if (!payload.IsValid)
				{
					TPayload payload2 = this.Payload;
					errors.AddRange(payload2.ValidationErrors);
				}
			}
			if (this.Recipient == null)
			{
				errors.Add(Strings.InvalidRecipient);
				return;
			}
			TRecipient recipient = this.Recipient;
			if (!recipient.IsValid)
			{
				TRecipient recipient2 = this.Recipient;
				errors.AddRange(recipient2.ValidationErrors);
			}
		}

		private class FragmentId<T> where T : BasicNotificationRecipient
		{
			public FragmentId(string notificationId, T recipient)
			{
				if (recipient != null)
				{
					this.AppId = recipient.AppId;
					this.RecipientId = recipient.DeviceId;
				}
				if (notificationId != null || this.AppId != null)
				{
					this.Identifier = string.Format("{0}-{1}", notificationId ?? string.Empty, this.AppId ?? string.Empty);
				}
				this.NotificationId = notificationId;
			}

			public string NotificationId { get; private set; }

			public string Identifier { get; private set; }

			public string AppId { get; private set; }

			public string RecipientId { get; private set; }
		}
	}
}
