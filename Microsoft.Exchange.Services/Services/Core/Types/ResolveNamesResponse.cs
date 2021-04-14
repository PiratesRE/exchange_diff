using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("ResolveNamesResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class ResolveNamesResponse : BaseInfoResponse
	{
		public ResolveNamesResponse() : base(ResponseType.ResolveNamesResponseMessage)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue value)
		{
			return new ResolveNamesResponseMessage(code, error, value as ResolutionSet);
		}
	}
}
