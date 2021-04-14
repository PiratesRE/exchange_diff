using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetImItemListResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetImItemListResponseMessage : ResponseMessage
	{
		public GetImItemListResponseMessage()
		{
		}

		internal GetImItemListResponseMessage(ServiceResultCode code, ServiceError error, ImItemList result) : base(code, error)
		{
			this.ImItemList = result;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetImItemListResponseMessage;
		}

		[XmlElement]
		[DataMember]
		public ImItemList ImItemList { get; set; }
	}
}
