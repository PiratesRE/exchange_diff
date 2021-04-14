using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetStreamingEventsResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlRoot(ElementName = "GetStreamingEventsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetStreamingEventsResponse : BaseInfoResponse
	{
		public GetStreamingEventsResponse() : base(ResponseType.GetStreamingEventsResponseMessage)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue value)
		{
			return new GetStreamingEventsResponseMessage(code, error);
		}

		internal override void ProcessServiceResult<TValue>(ServiceResult<TValue> result)
		{
			base.AddResponse(this.CreateResponseMessage<TValue>(result.Code, result.Error, result.Value));
		}
	}
}
