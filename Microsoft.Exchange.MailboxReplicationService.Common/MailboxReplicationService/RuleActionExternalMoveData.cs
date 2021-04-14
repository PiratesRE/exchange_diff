using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionExternalMoveData : RuleActionExternalMoveCopyData
	{
		public RuleActionExternalMoveData()
		{
		}

		public RuleActionExternalMoveData(RuleAction.ExternalMove ruleAction) : base(ruleAction)
		{
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.ExternalMove(base.StoreEntryID, base.FolderEntryID);
		}

		protected override string ToStringInternal()
		{
			return string.Format("EXTERNALMOVE {0}", base.ToStringInternal());
		}
	}
}
