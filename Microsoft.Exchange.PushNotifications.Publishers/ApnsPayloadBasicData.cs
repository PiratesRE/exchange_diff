using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal sealed class ApnsPayloadBasicData
	{
		public ApnsPayloadBasicData(int? badge = null, string sound = null, ApnsAlert alert = null, int contentAvailable = 0)
		{
			this.Badge = badge;
			this.Sound = sound;
			this.Alert = alert;
			this.ContentAvailable = contentAvailable;
		}

		[DataMember(Name = "badge", EmitDefaultValue = false, Order = 1)]
		public int? Badge { get; private set; }

		[DataMember(Name = "sound", EmitDefaultValue = false, Order = 2)]
		public string Sound { get; private set; }

		[DataMember(Name = "alert", EmitDefaultValue = false, Order = 3)]
		public ApnsAlert Alert { get; private set; }

		[DataMember(Name = "content-available", EmitDefaultValue = false, Order = 4)]
		public int ContentAvailable { get; private set; }

		public string ToJson()
		{
			return JsonConverter.Serialize<ApnsPayloadBasicData>(this, null);
		}

		public override string ToString()
		{
			if (this.toStringCache == null)
			{
				this.toStringCache = string.Format("{{badge:{0}; sound:{1}; alert:{2}; content-available:{3}}}", new object[]
				{
					this.Badge.ToNullableString<int>(),
					this.Sound.ToNullableString(),
					this.Alert.ToNullableString(null),
					this.ContentAvailable
				});
			}
			return this.toStringCache;
		}

		private string toStringCache;
	}
}
