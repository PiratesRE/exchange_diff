using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class OutlookNotificationPayload : BasicDataContract
	{
		public OutlookNotificationPayload(string tenantId = null, byte[] data = null)
		{
			this.TenantId = tenantId;
			if (data != null)
			{
				this.Data = Convert.ToBase64String(data, 0, data.Length);
			}
		}

		[DataMember(Name = "tenantId", EmitDefaultValue = false)]
		public string TenantId { get; private set; }

		[DataMember(Name = "data", EmitDefaultValue = false)]
		public string Data { get; private set; }

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("tenantId:").Append(this.TenantId.ToNullableString()).Append("; ");
			sb.Append("data:").Append(this.Data.ToNullableString()).Append("; ");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			Guid guid;
			if (!string.IsNullOrEmpty(this.TenantId) && !Guid.TryParse(this.TenantId, out guid))
			{
				errors.Add(Strings.InvalidTenantId(this.TenantId));
			}
			if (string.IsNullOrEmpty(this.Data))
			{
				errors.Add(Strings.OutlookInvalidPayloadData);
			}
		}
	}
}
