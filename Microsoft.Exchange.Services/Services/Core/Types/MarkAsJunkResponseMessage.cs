using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "MarkAsJunkResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class MarkAsJunkResponseMessage : ResponseMessage
	{
		public MarkAsJunkResponseMessage()
		{
		}

		internal MarkAsJunkResponseMessage(ServiceResultCode code, ServiceError error, ItemId movedItemId) : base(code, error)
		{
			this.MovedItemId = movedItemId;
		}

		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces Namespaces
		{
			get
			{
				return ResponseMessage.namespaces;
			}
			set
			{
			}
		}

		[DataMember]
		[XmlElement("MovedItemId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ItemId MovedItemId { get; set; }
	}
}
