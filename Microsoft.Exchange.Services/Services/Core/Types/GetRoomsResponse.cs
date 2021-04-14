using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetRoomsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetRoomsResponse : ResponseMessage
	{
		public GetRoomsResponse()
		{
		}

		internal GetRoomsResponse(ServiceResultCode code, ServiceError error, EwsRoomType[] rooms) : base(code, error)
		{
			this.Rooms = rooms;
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

		[XmlArrayItem("Room", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArray("Rooms", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public EwsRoomType[] Rooms { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetRoomsResponseMessage;
		}
	}
}
