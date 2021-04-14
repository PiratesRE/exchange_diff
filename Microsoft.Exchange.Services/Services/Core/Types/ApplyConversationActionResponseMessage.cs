using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ApplyConversationActionResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ApplyConversationActionResponseMessage : ResponseMessage
	{
		public ApplyConversationActionResponseMessage()
		{
		}

		internal ApplyConversationActionResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		[DataMember(EmitDefaultValue = false, Name = "MovedItemIds")]
		[XmlIgnore]
		public ItemId[] MovedItemIds { get; set; }
	}
}
