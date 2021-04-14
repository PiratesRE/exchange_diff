using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "RefreshSharingFolderResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class RefreshSharingFolderResponseMessage : ResponseMessage
	{
		public RefreshSharingFolderResponseMessage()
		{
		}

		internal RefreshSharingFolderResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.RefreshSharingFolderResponseMessage;
		}
	}
}
