using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal abstract class RuleActionExternalMoveCopyData : RuleActionMoveCopyData
	{
		public RuleActionExternalMoveCopyData()
		{
		}

		public RuleActionExternalMoveCopyData(RuleAction.MoveCopy ruleAction) : base(ruleAction)
		{
		}
	}
}
