using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetRoomListsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetRoomListsResponse : ResponseMessage
	{
		public GetRoomListsResponse()
		{
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

		internal GetRoomListsResponse(ServiceResultCode code, ServiceError error, EmailAddressWrapper[] roomLists) : base(code, error)
		{
			this.RoomLists = roomLists;
		}

		[XmlArrayItem("Address", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		[XmlArray("RoomLists", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public EmailAddressWrapper[] RoomLists { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetRoomListsResponseMessage;
		}
	}
}
