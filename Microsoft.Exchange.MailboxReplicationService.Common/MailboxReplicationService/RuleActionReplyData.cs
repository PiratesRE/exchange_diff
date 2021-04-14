using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionReplyData : RuleActionBaseReplyData
	{
		public RuleActionReplyData()
		{
		}

		public RuleActionReplyData(RuleAction.Reply ruleAction) : base(ruleAction, ruleAction.ReplyTemplateMessageEntryID, ruleAction.ReplyTemplateGuid, (uint)ruleAction.Flags)
		{
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.Reply(base.ReplyTemplateMessageEntryID, base.ReplyTemplateGuid, (RuleAction.Reply.ActionFlags)base.Flags);
		}

		protected override string ToStringInternal()
		{
			return string.Format("REPLY {0}", base.ToStringInternal());
		}
	}
}
