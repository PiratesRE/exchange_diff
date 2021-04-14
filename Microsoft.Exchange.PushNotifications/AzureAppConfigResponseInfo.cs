using System;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class AzureAppConfigResponseInfo : BasicDataContract
	{
		public AzureAppConfigResponseInfo(AzureAppConfigData[] data, string hubName)
		{
			this.AppData = data;
			this.HubName = hubName;
		}

		[DataMember(Name = "appData", EmitDefaultValue = false)]
		public AzureAppConfigData[] AppData { get; private set; }

		[DataMember(Name = "hubName", EmitDefaultValue = false)]
		public string HubName { get; private set; }

		public override string ToJson()
		{
			return JsonConverter.Serialize<AzureAppConfigResponseInfo>(this, null);
		}

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("hubName:").Append(this.HubName.ToNullableString()).Append("; ");
			sb.Append("appData:").Append(this.AppData.ToNullableString((AzureAppConfigData x) => x.ToFullString())).Append("; ");
		}
	}
}
