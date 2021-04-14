using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal abstract class BasicMulticastNotification<TPayload, TRecipient> : MulticastNotification where TPayload : BasicDataContract where TRecipient : BasicNotificationRecipient
	{
		public BasicMulticastNotification(TPayload payload, List<TRecipient> recipients)
		{
			this.Payload = payload;
			this.Recipients = recipients;
		}

		[DataMember(Name = "payload", EmitDefaultValue = false)]
		public TPayload Payload { get; private set; }

		[DataMember(Name = "recipients", EmitDefaultValue = false)]
		public List<TRecipient> Recipients { get; private set; }

		public override IEnumerable<Notification> GetFragments()
		{
			if (this.Recipients != null)
			{
				foreach (TRecipient recipient in this.Recipients)
				{
					yield return this.CreateFragment(this.Payload, recipient);
				}
			}
			yield break;
		}

		public void Validate()
		{
			if (!base.IsValid)
			{
				throw new InvalidNotificationException(base.ValidationErrors[0]);
			}
		}

		protected abstract Notification CreateFragment(TPayload payload, TRecipient recipient);

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("payload:").Append(this.Payload.ToNullableString((TPayload x) => x.ToFullString())).Append("; ");
			sb.Append("recipients:").Append(this.Recipients.ToNullableString((TRecipient x) => x.ToFullString())).Append("; ");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			if (this.Recipients == null || this.Recipients.Count <= 0)
			{
				errors.Add(Strings.InvalidRecipientsList);
			}
		}
	}
}
