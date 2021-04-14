using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("AddNewTelUriContactToGroupResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class AddNewTelUriContactToGroupResponseMessage : ResponseMessage
	{
		public AddNewTelUriContactToGroupResponseMessage()
		{
		}

		internal AddNewTelUriContactToGroupResponseMessage(ServiceResultCode code, ServiceError error, Persona result) : base(code, error)
		{
			this.Persona = result;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.AddNewTelUriContactToGroupResponseMessage;
		}

		[DataMember]
		[XmlElement]
		public Persona Persona { get; set; }
	}
}
