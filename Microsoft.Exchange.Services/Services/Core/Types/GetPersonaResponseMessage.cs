using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetPersonaResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetPersonaResponseMessage : ResponseMessage
	{
		public GetPersonaResponseMessage()
		{
		}

		internal GetPersonaResponseMessage(ServiceResultCode code, ServiceError error, GetPersonaResponseMessage response) : base(code, error)
		{
			this.persona = null;
			if (response != null)
			{
				this.persona = response.persona;
			}
		}

		[DataMember]
		[XmlElement("Persona")]
		public Persona Persona
		{
			get
			{
				return this.persona;
			}
			set
			{
				this.persona = value;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetPersonaResponseMessage;
		}

		private Persona persona;
	}
}
