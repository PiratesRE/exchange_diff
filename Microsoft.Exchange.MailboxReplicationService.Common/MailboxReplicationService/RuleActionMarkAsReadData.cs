using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionMarkAsReadData : RuleActionData
	{
		public RuleActionMarkAsReadData()
		{
		}

		public RuleActionMarkAsReadData(RuleAction.MarkAsRead ruleAction) : base(ruleAction)
		{
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.MarkAsRead();
		}

		protected override string ToStringInternal()
		{
			return "MARKASREAD";
		}
	}
}
