using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public abstract class BaseInfoResponse : BaseResponseMessage
	{
		internal BaseInfoResponse(ResponseType responseType) : base(responseType)
		{
		}

		internal virtual void ProcessServiceResult<TValue>(ServiceResult<TValue> result)
		{
			base.AddResponse(this.CreateResponseMessage<TValue>(result.Code, result.Error, result.Value));
		}

		internal abstract ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue item);

		internal void BuildForResults<TValue>(ServiceResult<TValue>[] serviceResults)
		{
			ServiceResult<TValue>.ProcessServiceResults(serviceResults, new ProcessServiceResult<TValue>(this.ProcessServiceResult<TValue>));
		}

		internal delegate BaseInfoResponse CreateBaseInfoResponse();
	}
}
