using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetConversationItemsDiagnosticsResponse : BaseInfoResponse
	{
		public GetConversationItemsDiagnosticsResponse() : base(ResponseType.GetConversationItemsDiagnosticsResponseMessage)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue item)
		{
			return new GetConversationItemsDiagnosticsResponseMessage(code, error, item as GetConversationItemsDiagnosticsResponseType);
		}
	}
}
