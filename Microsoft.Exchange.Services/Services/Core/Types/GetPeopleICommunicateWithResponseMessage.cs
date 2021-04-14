using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "GetPeopleICommunicateWithResponseMessage", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetPeopleICommunicateWithResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public sealed class GetPeopleICommunicateWithResponseMessage : ResponseMessage
	{
		public GetPeopleICommunicateWithResponseMessage()
		{
		}

		internal GetPeopleICommunicateWithResponseMessage(ServiceResultCode code, ServiceError error, Stream stream) : base(code, error)
		{
			this.Stream = stream;
		}

		internal Stream Stream { get; private set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetPeopleICommunicateWithResponseMessage;
		}
	}
}
