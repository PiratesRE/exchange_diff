using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetTimeZoneOffsetsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetTimeZoneOffsetsResponseMessage : ResponseMessage
	{
		public GetTimeZoneOffsetsResponseMessage()
		{
		}

		internal GetTimeZoneOffsetsResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetTimeZoneOffsetsResponseMessage;
		}

		[XmlArray(ElementName = "TimeZones")]
		[XmlArrayItem(ElementName = "TimeZone", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public TimeZoneOffsetsType[] TimeZones { get; set; }
	}
}
