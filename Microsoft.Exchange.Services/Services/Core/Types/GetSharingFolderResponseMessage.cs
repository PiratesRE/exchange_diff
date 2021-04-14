using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "GetSharingFolderResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetSharingFolderResponseMessage : ResponseMessage
	{
		public GetSharingFolderResponseMessage()
		{
		}

		internal GetSharingFolderResponseMessage(ServiceResultCode code, ServiceError error, XmlElement sharingFolderId) : base(code, error)
		{
			this.SharingFolderId = sharingFolderId;
		}

		[XmlAnyElement]
		public XmlElement SharingFolderId { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetSharingFolderResponseMessage;
		}
	}
}
