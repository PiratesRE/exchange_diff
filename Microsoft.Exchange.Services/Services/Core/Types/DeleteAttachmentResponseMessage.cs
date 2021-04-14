using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("DeleteAttachmentResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class DeleteAttachmentResponseMessage : ResponseMessage
	{
		public DeleteAttachmentResponseMessage()
		{
		}

		internal DeleteAttachmentResponseMessage(ServiceResultCode code, ServiceError error, RootItemIdType rootItemId) : base(code, error)
		{
			this.RootItemId = rootItemId;
		}

		[DataMember]
		[XmlElement("RootItemId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public RootItemIdType RootItemId { get; set; }
	}
}
