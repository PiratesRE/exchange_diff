using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[KnownType(typeof(InboxRule))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetInboxRuleRequest : UpdateInboxRuleRequestBase
	{
		[DataMember(IsRequired = true)]
		public SetInboxRuleData InboxRule { get; set; }

		public override string ToString()
		{
			return string.Format(base.ToString() + ", InboxRule = {0}", this.InboxRule);
		}
	}
}
