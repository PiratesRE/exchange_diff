using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class RemoteUserNotificationPayload : UserNotificationPayload
	{
		public RemoteUserNotificationPayload(string notificationType, string data = null) : base(notificationType, data)
		{
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

		public void SetUserId(string userId)
		{
			this.userId = userId;
		}

		public void SetTenantId(string tenantId)
		{
			this.tenantId = tenantId;
		}

		public const string UserIdHeader = "X-PUN-UserId";

		public const string TenantIdHeader = "X-PUN-TenantId";

		private string userId;

		private string tenantId;
	}
}
