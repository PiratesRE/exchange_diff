using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class ApnsPayload
	{
		public ApnsPayload(ApnsPayloadBasicData aps, string storeObjectId = null, string backgroundSyncType = null)
		{
			this.Aps = aps;
			this.StoreObjectId = storeObjectId;
			this.BackgroundSyncType = backgroundSyncType;
		}

		[DataMember(Name = "aps", EmitDefaultValue = false, IsRequired = true, Order = 1)]
		public ApnsPayloadBasicData Aps { get; private set; }

		[DataMember(Name = "o", EmitDefaultValue = false, IsRequired = false, Order = 2)]
		public string StoreObjectId { get; private set; }

		[DataMember(Name = "t", EmitDefaultValue = false, IsRequired = false, Order = 3)]
		public string BackgroundSyncType { get; private set; }

		[DataMember(Name = "n", EmitDefaultValue = false, IsRequired = false, Order = 4)]
		public string NotificationId { get; internal set; }

		public string ToJson()
		{
			return JsonConverter.Serialize<ApnsPayload>(this, null);
		}

		public override string ToString()
		{
			if (this.toStringCache == null)
			{
				this.toStringCache = string.Format("{{aps:{0}, o:{1}, t:{2}, n:{3}}}", new object[]
				{
					this.Aps.ToNullableString(null),
					this.StoreObjectId.ToNullableString(),
					this.BackgroundSyncType.ToNullableString(),
					this.NotificationId.ToNullableString()
				});
			}
			return this.toStringCache;
		}

		private string toStringCache;
	}
}
