using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(AggregatedAccountType))]
	[XmlInclude(typeof(AggregatedAccountType))]
	[XmlType("SetAggregatedAccountResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SetAggregatedAccountResponse : ResponseMessage
	{
		public SetAggregatedAccountResponse()
		{
		}

		internal SetAggregatedAccountResponse(ServiceResultCode code, ServiceError error, AggregatedAccountType account) : base(code, error)
		{
			this.Account = account;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SetAggregatedAccountResponseMessage;
		}

		[XmlElement("Account")]
		public AggregatedAccountType Account { get; set; }
	}
}
