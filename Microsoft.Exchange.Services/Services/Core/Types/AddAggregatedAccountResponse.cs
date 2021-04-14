using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(AggregatedAccountType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlInclude(typeof(AggregatedAccountType))]
	[XmlType("AddAggregatedAccountResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class AddAggregatedAccountResponse : ResponseMessage
	{
		public AddAggregatedAccountResponse()
		{
		}

		internal AddAggregatedAccountResponse(ServiceResultCode code, ServiceError error, AggregatedAccountType account) : base(code, error)
		{
			this.Account = account;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.AddAggregatedAccountResponseMessage;
		}

		[XmlElement("Account")]
		[DataMember]
		public AggregatedAccountType Account { get; set; }
	}
}
