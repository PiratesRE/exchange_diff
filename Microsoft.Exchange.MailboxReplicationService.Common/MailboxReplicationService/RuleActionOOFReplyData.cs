using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionOOFReplyData : RuleActionBaseReplyData
	{
		public RuleActionOOFReplyData()
		{
		}

		public RuleActionOOFReplyData(RuleAction.OOFReply ruleAction) : base(ruleAction, ruleAction.ReplyTemplateMessageEntryID, ruleAction.ReplyTemplateGuid, 0U)
		{
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.OOFReply(base.ReplyTemplateMessageEntryID, base.ReplyTemplateGuid);
		}

		protected override string ToStringInternal()
		{
			return string.Format("OOFREPLY {0}", base.ToStringInternal());
		}
	}
}
