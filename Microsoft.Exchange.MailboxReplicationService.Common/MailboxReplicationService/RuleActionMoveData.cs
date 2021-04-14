using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionMoveData : RuleActionMoveCopyData
	{
		public RuleActionMoveData()
		{
		}

		public RuleActionMoveData(RuleAction.MoveCopy ruleAction) : base(ruleAction)
		{
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.MoveCopy(RuleAction.Type.OP_MOVE, base.StoreEntryID, base.FolderEntryID);
		}

		protected override string ToStringInternal()
		{
			return string.Format("MOVE {0}", base.ToStringInternal());
		}
	}
}
