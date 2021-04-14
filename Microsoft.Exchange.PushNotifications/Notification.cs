using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal abstract class Notification : BasicNotification
	{
		public Notification(string appId, string recipientId, string identifier = null) : base(identifier)
		{
			this.AppId = appId;
			this.RecipientId = recipientId;
		}

		[DataMember(Name = "appId", EmitDefaultValue = false)]
		public string AppId { get; private set; }

		[DataMember(Name = "recipientId", EmitDefaultValue = false)]
		public string RecipientId { get; private set; }

		[DataMember(Name = "isMonitoring")]
		public bool IsMonitoring { get; protected set; }

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("appId:").Append(this.AppId.ToNullableString()).Append("; ");
			sb.Append("recipientId:").Append(this.RecipientId.ToNullableString()).Append("; ");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			if (string.IsNullOrWhiteSpace(this.AppId))
			{
				errors.Add(Strings.InvalidRecipientAppId);
			}
			if (string.IsNullOrWhiteSpace(this.RecipientId))
			{
				errors.Add(Strings.InvalidRecipientDeviceId);
			}
		}
	}
}
