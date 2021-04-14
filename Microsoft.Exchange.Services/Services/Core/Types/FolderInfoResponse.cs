using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class FolderInfoResponse : BaseInfoResponse
	{
		internal FolderInfoResponse(ResponseType responseType) : base(responseType)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue value)
		{
			return new FolderInfoResponseMessage(code, error, value as BaseFolderType);
		}
	}
}
