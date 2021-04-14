using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal abstract class UserNotification<TPayload> : BasicMulticastNotification<TPayload, UserNotificationRecipient> where TPayload : UserNotificationPayload
	{
		public UserNotification(string workloadId, TPayload payload, List<UserNotificationRecipient> recipients) : base(payload, recipients)
		{
			this.WorkloadId = workloadId;
		}

		[DataMember(Name = "workloadId", EmitDefaultValue = false)]
		public string WorkloadId { get; private set; }

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("workloadId:").Append(this.WorkloadId.ToNullableString()).Append("; ");
		}
	}
}
