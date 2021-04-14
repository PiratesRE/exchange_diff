using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetNonIndexableItemDetailsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetNonIndexableItemDetailsResponse : ResponseMessage
	{
		public GetNonIndexableItemDetailsResponse()
		{
		}

		internal GetNonIndexableItemDetailsResponse(ServiceResultCode code, ServiceError error, NonIndexableItemDetailResult result) : base(code, error)
		{
			this.NonIndexableItemDetailsResult = result;
		}

		[XmlElement("NonIndexableItemDetailsResult")]
		[DataMember(Name = "NonIndexableItemDetailsResult", IsRequired = false)]
		public NonIndexableItemDetailResult NonIndexableItemDetailsResult { get; set; }
	}
}
