using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionDelegateData : RuleActionFwdDelegateData
	{
		public RuleActionDelegateData()
		{
		}

		public RuleActionDelegateData(RuleAction.Delegate ruleAction) : base(ruleAction, 0U)
		{
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.Delegate(DataConverter<AdrEntryConverter, AdrEntry, AdrEntryData>.GetNative(base.Recipients));
		}

		protected override string ToStringInternal()
		{
			return string.Format("DELEGATE {0}", base.ToStringInternal());
		}
	}
}
