using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("RemoveAggregatedAccountResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class RemoveAggregatedAccountResponse : ResponseMessage
	{
		public RemoveAggregatedAccountResponse()
		{
		}

		internal RemoveAggregatedAccountResponse(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.RemoveAggregatedAccountResponseMessage;
		}
	}
}
