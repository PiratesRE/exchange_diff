using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("DeleteAttachmentResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class DeleteAttachmentResponse : BaseInfoResponse
	{
		public DeleteAttachmentResponse() : base(ResponseType.DeleteAttachmentResponseMessage)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue value)
		{
			return new DeleteAttachmentResponseMessage(code, error, value as RootItemIdType);
		}
	}
}
