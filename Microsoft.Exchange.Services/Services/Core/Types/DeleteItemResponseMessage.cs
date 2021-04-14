using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DeleteItemResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class DeleteItemResponseMessage : ResponseMessage
	{
		public DeleteItemResponseMessage()
		{
		}

		internal DeleteItemResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Name = "MovedItemId")]
		public ItemId MovedItemId { get; set; }
	}
}
