using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetClutterStateResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetClutterStateResponse : ResponseMessage
	{
		[DataMember]
		[XmlElement(ElementName = "ClutterState")]
		public ClutterState ClutterState { get; set; }

		public GetClutterStateResponse()
		{
		}

		internal GetClutterStateResponse(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetClutterStateResponseMessage;
		}
	}
}
