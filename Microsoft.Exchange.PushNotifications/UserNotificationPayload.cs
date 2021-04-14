using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal abstract class UserNotificationPayload : BasicDataContract
	{
		public UserNotificationPayload(string notificationType, string data = null)
		{
			this.NotificationType = notificationType;
			this.Data = data;
		}

		[DataMember(Name = "notificationType", EmitDefaultValue = false)]
		public string NotificationType { get; private set; }

		[DataMember(Name = "data", EmitDefaultValue = false)]
		public string Data { get; private set; }

		public abstract string UserId { get; }

		public abstract string TenantId { get; }

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("notificationType:").Append(this.NotificationType.ToNullableString()).Append("; ");
			sb.Append("data:").Append(this.Data.ToNullableString()).Append("; ");
			sb.Append("userId:").Append(this.UserId.ToNullableString()).Append("; ");
			sb.Append("tenantId:").Append(this.TenantId.ToNullableString()).Append("; ");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			Guid guid;
			if (!string.IsNullOrEmpty(this.TenantId) && !Guid.TryParse(this.TenantId, out guid))
			{
				errors.Add(Strings.InvalidTenantId(this.TenantId));
			}
		}
	}
}
