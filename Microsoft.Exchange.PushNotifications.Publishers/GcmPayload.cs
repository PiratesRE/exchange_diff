using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class GcmPayload
	{
		public GcmPayload(int? unseenEmailCount = null, string message = null, string extraData = null, BackgroundSyncType backgroundSyncType = BackgroundSyncType.None)
		{
			this.UnseenEmailCount = unseenEmailCount;
			this.Message = message;
			this.ExtraData = extraData;
			this.BackgroundSyncType = backgroundSyncType;
		}

		public int? UnseenEmailCount { get; private set; }

		public string Message { get; private set; }

		public string ExtraData { get; private set; }

		public BackgroundSyncType BackgroundSyncType { get; private set; }

		public string NotificationId { get; internal set; }

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Format("{{unseenEmailCount:{0}; message:{1}; data:{2}; backgroundSyncType:{3}; notificationId:{4}}}", new object[]
				{
					this.UnseenEmailCount.ToNullableString<int>(),
					this.Message.ToNullableString(),
					this.ExtraData.ToNullableString(),
					this.BackgroundSyncType.ToNullableString(null),
					this.NotificationId.ToNullableString()
				});
			}
			return this.toString;
		}

		internal void WriteGcmPayload(GcmPayloadWriter gpw)
		{
			ArgumentValidator.ThrowIfNull("gpw", gpw);
			gpw.WriteProperty<int>("data.UnseenEmailCount", this.UnseenEmailCount);
			gpw.WriteProperty("data.Message", this.Message);
			gpw.WriteProperty("data.ExtraData", this.ExtraData);
			gpw.WriteProperty<int>("data.BackgroundSyncType", (int)this.BackgroundSyncType);
			gpw.WriteProperty("data.ServerNotificationId", this.NotificationId);
		}

		private string toString;
	}
}
