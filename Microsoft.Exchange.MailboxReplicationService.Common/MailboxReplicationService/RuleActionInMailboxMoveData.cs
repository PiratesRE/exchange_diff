using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionInMailboxMoveData : RuleActionInMailboxMoveCopyData
	{
		public RuleActionInMailboxMoveData()
		{
		}

		public RuleActionInMailboxMoveData(RuleAction.InMailboxMove ruleAction) : base(ruleAction)
		{
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.InMailboxMove(base.FolderEntryID);
		}

		protected override string ToStringInternal()
		{
			return string.Format("INMAILBOXMOVE {0}", base.ToStringInternal());
		}
	}
}
