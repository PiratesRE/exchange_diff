using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("SetClutterStateResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SetClutterStateResponse : ResponseMessage
	{
		[XmlElement(ElementName = "ClutterState")]
		[DataMember]
		public ClutterState ClutterState { get; set; }

		public SetClutterStateResponse()
		{
		}

		internal SetClutterStateResponse(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SetClutterStateResponseMessage;
		}
	}
}
