using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "PerformInstantSearchResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public sealed class PerformInstantSearchResponse : ResponseMessage
	{
		public PerformInstantSearchResponse() : this(null)
		{
		}

		public PerformInstantSearchResponse(InstantSearchPayloadType payload)
		{
			this.Payload = payload;
		}

		[DataMember]
		public InstantSearchPayloadType Payload { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.PerformInstantSearchResponseMessage;
		}
	}
}
