using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionDeleteData : RuleActionData
	{
		public RuleActionDeleteData()
		{
		}

		public RuleActionDeleteData(RuleAction.Delete ruleAction) : base(ruleAction)
		{
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.Delete();
		}

		protected override string ToStringInternal()
		{
			return "DELETE";
		}
	}
}
