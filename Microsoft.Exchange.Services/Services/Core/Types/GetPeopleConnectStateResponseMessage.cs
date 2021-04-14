using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "GetPeopleConnectStateResponseMessage", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetPeopleConnectStateResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public sealed class GetPeopleConnectStateResponseMessage : ResponseMessage
	{
		public GetPeopleConnectStateResponseMessage()
		{
		}

		internal GetPeopleConnectStateResponseMessage(ServiceResultCode code, ServiceError error, PeopleConnectionState result) : base(code, error)
		{
			this.PeopleConnectionState = result;
		}

		[DataMember]
		[XmlElement]
		public PeopleConnectionState PeopleConnectionState { get; set; }
	}
}
