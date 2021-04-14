using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetClientAccessTokenResponseMessage : ResponseMessage
	{
		public GetClientAccessTokenResponseMessage()
		{
		}

		internal GetClientAccessTokenResponseMessage(ServiceResultCode code, ServiceError error, ClientAccessTokenResponseType token) : base(code, error)
		{
			this.Token = token;
		}

		[DataMember]
		[XmlElement(ElementName = "Token", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ClientAccessTokenResponseType Token { get; set; }
	}
}
