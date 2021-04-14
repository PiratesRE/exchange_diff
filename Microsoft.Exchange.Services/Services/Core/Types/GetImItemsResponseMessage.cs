using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetImItemsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetImItemsResponseMessage : ResponseMessage
	{
		public GetImItemsResponseMessage()
		{
		}

		internal GetImItemsResponseMessage(ServiceResultCode code, ServiceError error, ImItemList result) : base(code, error)
		{
			this.ImItemList = result;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetImItemsResponseMessage;
		}

		[XmlElement]
		[DataMember]
		public ImItemList ImItemList { get; set; }
	}
}
