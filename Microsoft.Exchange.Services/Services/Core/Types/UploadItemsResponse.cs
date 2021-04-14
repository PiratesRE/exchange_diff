using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UploadItemsResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class UploadItemsResponse : BaseInfoResponse
	{
		public UploadItemsResponse() : base(ResponseType.UploadItemsResponseMessage)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue value)
		{
			return new UploadItemsResponseMessage(code, error, value as XmlNode);
		}
	}
}
