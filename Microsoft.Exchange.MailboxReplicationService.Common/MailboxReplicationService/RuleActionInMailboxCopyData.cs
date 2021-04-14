using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionInMailboxCopyData : RuleActionInMailboxMoveCopyData
	{
		public RuleActionInMailboxCopyData()
		{
		}

		public RuleActionInMailboxCopyData(RuleAction.InMailboxCopy ruleAction) : base(ruleAction)
		{
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.InMailboxCopy(base.FolderEntryID);
		}

		protected override string ToStringInternal()
		{
			return string.Format("INMAILBOXCOPY {0}", base.ToStringInternal());
		}
	}
}
