using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UpdateInboxRulesResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class UpdateInboxRulesResponse : ResponseMessage
	{
		[XmlArrayItem("RuleOperationError", Type = typeof(RuleOperationError), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArray]
		public RuleOperationError[] RuleOperationErrors { get; set; }

		public UpdateInboxRulesResponse()
		{
		}

		internal UpdateInboxRulesResponse(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}
	}
}
