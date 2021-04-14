using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionExternalCopyData : RuleActionExternalMoveCopyData
	{
		public RuleActionExternalCopyData()
		{
		}

		public RuleActionExternalCopyData(RuleAction.ExternalCopy ruleAction) : base(ruleAction)
		{
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.ExternalCopy(base.StoreEntryID, base.FolderEntryID);
		}

		protected override string ToStringInternal()
		{
			return string.Format("EXTERNALCOPY {0}", base.ToStringInternal());
		}
	}
}
