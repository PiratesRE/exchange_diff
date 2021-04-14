using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class MailboxNotificationPayload : BasicDataContract
	{
		public MailboxNotificationPayload(string tenantId = null, int? unseenEmailCount = null, BackgroundSyncType backgroundSyncType = BackgroundSyncType.None, string language = null)
		{
			this.TenantId = tenantId;
			this.UnseenEmailCount = unseenEmailCount;
			this.BackgroundSyncType = backgroundSyncType;
			this.Language = language;
		}

		[DataMember(Name = "tenantId", EmitDefaultValue = false)]
		public string TenantId { get; private set; }

		[DataMember(Name = "unseenEmailCount", EmitDefaultValue = false)]
		public int? UnseenEmailCount { get; private set; }

		[DataMember(Name = "language", EmitDefaultValue = false)]
		public string Language { get; private set; }

		[DataMember(Name = "backgroundSyncType", EmitDefaultValue = false)]
		public BackgroundSyncType BackgroundSyncType { get; private set; }

		[DataMember(Name = "isMonitoring", EmitDefaultValue = false)]
		public bool IsMonitoring { get; private set; }

		internal static MailboxNotificationPayload CreateMonitoringPayload(string monitoringTenantId = "")
		{
			return new MailboxNotificationPayload(monitoringTenantId, new int?(1), BackgroundSyncType.None, null)
			{
				IsMonitoring = true
			};
		}

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("tenantId:").Append(this.TenantId.ToNullableString()).Append("; ");
			sb.Append("unseenEmailCount:").Append(this.UnseenEmailCount.ToNullableString<int>()).Append("; ");
			sb.Append("language:").Append(this.Language.ToNullableString()).Append("; ");
			sb.Append("backgroundSyncType:").Append(this.BackgroundSyncType.ToString()).Append("; ");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			if (this.UnseenEmailCount == null && this.BackgroundSyncType != BackgroundSyncType.None)
			{
				errors.Add(Strings.InvalidMnPayloadContent);
			}
			Guid guid;
			if (!string.IsNullOrEmpty(this.TenantId) && !Guid.TryParse(this.TenantId, out guid))
			{
				errors.Add(Strings.InvalidTenantId(this.TenantId));
			}
		}
	}
}
