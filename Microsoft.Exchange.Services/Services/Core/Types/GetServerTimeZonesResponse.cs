using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetServerTimeZonesResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetServerTimeZonesResponse : BaseInfoResponse
	{
		public GetServerTimeZonesResponse() : base(ResponseType.GetServerTimeZonesResponseMessage)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue timeZones)
		{
			return new GetServerTimeZonesResponseMessage(code, error, timeZones as GetServerTimeZoneResultType);
		}
	}
}
