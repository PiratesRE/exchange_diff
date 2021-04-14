using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetInboxRuleResponse : OptionsResponseBase
	{
		public GetInboxRuleResponse()
		{
			this.InboxRuleCollection = new InboxRuleCollection();
		}

		[DataMember(IsRequired = true)]
		public InboxRuleCollection InboxRuleCollection { get; set; }

		public override string ToString()
		{
			return string.Format("GetInboxRuleResponse: {0}", this.InboxRuleCollection);
		}
	}
}
