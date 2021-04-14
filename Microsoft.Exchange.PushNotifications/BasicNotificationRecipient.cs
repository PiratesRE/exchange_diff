using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class BasicNotificationRecipient : BasicDataContract
	{
		public BasicNotificationRecipient(string appId, string deviceId)
		{
			this.AppId = appId;
			this.DeviceId = deviceId;
		}

		[DataMember(Name = "appId", EmitDefaultValue = false, IsRequired = true)]
		public string AppId { get; private set; }

		[DataMember(Name = "deviceId", EmitDefaultValue = false, IsRequired = true)]
		public string DeviceId { get; private set; }

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("appId:").Append(this.AppId.ToNullableString()).Append("; ");
			sb.Append("deviceId:").Append(this.DeviceId.ToNullableString()).Append("; ");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			if (string.IsNullOrWhiteSpace(this.AppId))
			{
				errors.Add(Strings.InvalidRecipientAppId);
			}
			if (string.IsNullOrWhiteSpace(this.DeviceId))
			{
				errors.Add(Strings.InvalidRecipientDeviceId);
			}
		}
	}
}
