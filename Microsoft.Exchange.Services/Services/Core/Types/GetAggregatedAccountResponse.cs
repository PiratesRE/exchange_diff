using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetAggregatedAccountResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetAggregatedAccountResponse : ResponseMessage
	{
		public GetAggregatedAccountResponse()
		{
		}

		internal GetAggregatedAccountResponse(ServiceResultCode code, ServiceError error, AggregatedAccountType[] aggregatedAccounts) : base(code, error)
		{
			this.AggregatedAccounts = aggregatedAccounts;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetAggregatedAccountResponseMessage;
		}

		[XmlArrayItem("AggregatedAccounts", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArray("AggregatedAccounts", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember]
		public AggregatedAccountType[] AggregatedAccounts { get; set; }
	}
}
