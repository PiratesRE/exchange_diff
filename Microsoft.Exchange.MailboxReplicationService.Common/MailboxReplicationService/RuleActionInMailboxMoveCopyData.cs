using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal abstract class RuleActionInMailboxMoveCopyData : RuleActionMoveCopyData
	{
		public RuleActionInMailboxMoveCopyData()
		{
		}

		private new byte[] StoreEntryID { get; set; }

		public RuleActionInMailboxMoveCopyData(RuleAction.MoveCopy ruleAction) : base(ruleAction)
		{
		}

		protected override string ToStringInternal()
		{
			return string.Format("FolderEID:{0}", TraceUtils.DumpEntryId(base.FolderEntryID));
		}
	}
}
