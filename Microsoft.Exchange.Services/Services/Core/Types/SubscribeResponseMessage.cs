using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("SubscribeResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SubscribeResponseMessage : ResponseMessage
	{
		public SubscribeResponseMessage()
		{
		}

		internal SubscribeResponseMessage(ServiceResultCode code, ServiceError error, SubscriptionBase value) : base(code, error)
		{
			if (value != null)
			{
				this.SubscriptionId = value.SubscriptionId;
				if (value.UseWatermarks)
				{
					this.Watermark = value.LastWatermarkSent;
				}
			}
		}

		[DataMember(Name = "SubscriptionId", EmitDefaultValue = false)]
		[XmlElement("SubscriptionId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string SubscriptionId { get; set; }

		[DataMember(Name = "Watermark", EmitDefaultValue = false)]
		[XmlElement("Watermark", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string Watermark { get; set; }
	}
}
