using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetLikersResponseMessage : ResponseMessage
	{
		public GetLikersResponseMessage()
		{
		}

		internal GetLikersResponseMessage(ServiceResultCode code, ServiceError error, GetLikersResponseMessage response) : base(code, error)
		{
			if (response != null)
			{
				this.personas = response.personas;
			}
		}

		[DataMember]
		public Persona[] Personas
		{
			get
			{
				return this.personas;
			}
			set
			{
				this.personas = value;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetLikersResponseMessage;
		}

		private Persona[] personas;
	}
}
