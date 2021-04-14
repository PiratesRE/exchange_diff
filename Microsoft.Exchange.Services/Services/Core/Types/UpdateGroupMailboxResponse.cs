using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UpdateGroupMailboxResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class UpdateGroupMailboxResponse : BaseResponseMessage
	{
		public UpdateGroupMailboxResponse() : base(ResponseType.UpdateGroupMailboxResponseMessage)
		{
		}
	}
}
