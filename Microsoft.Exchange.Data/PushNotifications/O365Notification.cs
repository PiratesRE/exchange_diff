using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Data.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class O365Notification
	{
		public O365Notification(string channelId, string data)
		{
			this.ChannelId = channelId;
			this.Data = data;
		}

		[DataMember(Name = "channelId", EmitDefaultValue = false, IsRequired = true)]
		public string ChannelId { get; private set; }

		[DataMember(Name = "data", EmitDefaultValue = false, IsRequired = true)]
		public string Data { get; private set; }

		public string ToJson()
		{
			return JsonConverter.Serialize<O365Notification>(this, null);
		}

		public override string ToString()
		{
			return string.Format("channelId:{0}; data:{1}", this.ChannelId ?? "<null>", this.Data ?? "<null>");
		}

		public const string MonitoringChannelId = "::AE82E53440744F2798C276818CE8BD5C::";
	}
}
