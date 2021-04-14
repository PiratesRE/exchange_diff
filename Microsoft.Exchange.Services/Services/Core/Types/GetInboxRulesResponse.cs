using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetInboxRulesResponse : ResponseMessage
	{
		[DataMember(Order = 1)]
		[XmlElement]
		public bool OutlookRuleBlobExists { get; set; }

		[XmlArrayItem("Rule", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Order = 2)]
		[XmlArray]
		public EwsRule[] InboxRules { get; set; }

		public GetInboxRulesResponse()
		{
		}

		internal GetInboxRulesResponse(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}
	}
}
