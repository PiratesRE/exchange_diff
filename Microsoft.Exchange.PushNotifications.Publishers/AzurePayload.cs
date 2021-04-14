using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzurePayload
	{
		public AzurePayload(int? unseenEmailCount = null, string message = null, string storeObjectId = null, string backgroundSyncType = null)
		{
			this.UnseenEmailCount = unseenEmailCount;
			this.Message = message;
			this.StoreObjectId = storeObjectId;
			this.BackgroundSyncType = backgroundSyncType;
			this.IsBackground = (backgroundSyncType != null);
		}

		public string NotificationId { get; internal set; }

		public int? UnseenEmailCount { get; private set; }

		public string Message { get; private set; }

		public string StoreObjectId { get; private set; }

		public bool IsBackground { get; private set; }

		public string BackgroundSyncType { get; private set; }

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Format("{{id:{0}; unseen:{1}; message:{2}; storeId:{3}; isBackground:{4}; syncType:{5}}}", new object[]
				{
					this.NotificationId,
					this.UnseenEmailCount.ToNullableString<int>(),
					this.Message.ToNullableString(),
					this.StoreObjectId.ToNullableString(),
					this.IsBackground,
					this.BackgroundSyncType
				});
			}
			return this.toString;
		}

		internal void WriteAzurePayload(AzurePayloadWriter apw)
		{
			ArgumentValidator.ThrowIfNull("apw", apw);
			apw.WriteProperty("id", this.NotificationId);
			apw.WriteProperty<int>("unseen", this.UnseenEmailCount);
			apw.WriteProperty("message", this.Message);
			apw.WriteProperty("storeId", this.StoreObjectId);
			apw.WriteProperty<int>("background", this.IsBackground ? 1 : 0);
			apw.WriteProperty("syncType", this.BackgroundSyncType);
		}

		private string toString;
	}
}
