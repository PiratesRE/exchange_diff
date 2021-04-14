using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetPeopleConnectTokenResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Name = "GetPeopleConnectTokenResponseMessage", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public sealed class GetPeopleConnectTokenResponseMessage : ResponseMessage
	{
		public GetPeopleConnectTokenResponseMessage()
		{
		}

		internal GetPeopleConnectTokenResponseMessage(ServiceResultCode code, ServiceError error, PeopleConnectionToken result) : base(code, error)
		{
			this.PeopleConnectionToken = result;
		}

		[XmlElement]
		[DataMember]
		public PeopleConnectionToken PeopleConnectionToken { get; set; }
	}
}
