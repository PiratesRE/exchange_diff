using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionCopyData : RuleActionMoveCopyData
	{
		public RuleActionCopyData()
		{
		}

		public RuleActionCopyData(RuleAction.MoveCopy ruleAction) : base(ruleAction)
		{
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.MoveCopy(RuleAction.Type.OP_COPY, base.StoreEntryID, base.FolderEntryID);
		}

		protected override string ToStringInternal()
		{
			return string.Format("COPY {0}", base.ToStringInternal());
		}
	}
}
