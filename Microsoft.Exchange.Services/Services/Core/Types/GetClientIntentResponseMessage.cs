using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetClientIntentResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public sealed class GetClientIntentResponseMessage : ResponseMessage
	{
		[DataMember(Name = "ClientIntent")]
		[XmlElement("ClientIntent")]
		public ClientIntent ClientIntent { get; set; }

		public GetClientIntentResponseMessage()
		{
		}

		internal GetClientIntentResponseMessage(ServiceResultCode code, ServiceError error, GetClientIntentResponseMessage result) : base(code, error)
		{
			if (result != null)
			{
				this.ClientIntent = result.ClientIntent;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetClientIntentResponseMessage;
		}
	}
}
