using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class LocalUserNotificationPayload : UserNotificationPayload
	{
		public LocalUserNotificationPayload(string notificationType, string data = null, string userId = null, string tenantId = null) : base(notificationType, data)
		{
			this.userId = userId;
			this.tenantId = tenantId;
		}

		public override string UserId
		{
			get
			{
				return this.userId;
			}
		}

		public override string TenantId
		{
			get
			{
				return this.tenantId;
			}
		}

		[DataMember(Name = "userId", EmitDefaultValue = false)]
		private readonly string userId;

		[DataMember(Name = "tenantId", EmitDefaultValue = false)]
		private readonly string tenantId;
	}
}
