using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("AddNewImContactToGroupResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class AddNewImContactToGroupResponseMessage : ResponseMessage
	{
		public AddNewImContactToGroupResponseMessage()
		{
		}

		internal AddNewImContactToGroupResponseMessage(ServiceResultCode code, ServiceError error, Persona result) : base(code, error)
		{
			this.Persona = result;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.AddNewImContactToGroupResponseMessage;
		}

		[XmlElement]
		[DataMember]
		public Persona Persona { get; set; }
	}
}
