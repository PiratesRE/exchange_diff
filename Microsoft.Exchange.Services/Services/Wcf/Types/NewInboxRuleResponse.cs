using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class NewInboxRuleResponse : OptionsResponseBase
	{
		[DataMember(IsRequired = true)]
		public InboxRule InboxRule { get; set; }

		public override string ToString()
		{
			return string.Format("NewInboxRuleResponse: {0}", this.InboxRule);
		}
	}
}
