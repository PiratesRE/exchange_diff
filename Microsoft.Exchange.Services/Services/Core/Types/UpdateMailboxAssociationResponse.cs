using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UpdateMailboxAssociationResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class UpdateMailboxAssociationResponse : BaseResponseMessage
	{
		public UpdateMailboxAssociationResponse() : base(ResponseType.UpdateMailboxAssociationResponseMessage)
		{
		}
	}
}
