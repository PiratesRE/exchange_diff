using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "PostModernGroupItemResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class PostModernGroupItemResponse : BaseInfoResponse
	{
		public PostModernGroupItemResponse() : base(ResponseType.PostModernGroupItemResponseMessage)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue item)
		{
			return new ItemInfoResponseMessage(code, error, item as ItemType[]);
		}
	}
}
