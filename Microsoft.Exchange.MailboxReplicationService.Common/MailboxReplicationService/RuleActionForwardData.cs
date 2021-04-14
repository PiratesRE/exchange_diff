using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionForwardData : RuleActionFwdDelegateData
	{
		public RuleActionForwardData()
		{
		}

		public RuleActionForwardData(RuleAction.Forward ruleAction) : base(ruleAction, (uint)ruleAction.Flags)
		{
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.Forward(DataConverter<AdrEntryConverter, AdrEntry, AdrEntryData>.GetNative(base.Recipients), (RuleAction.Forward.ActionFlags)base.Flags);
		}

		protected override string ToStringInternal()
		{
			return string.Format("FORWARD {0}", base.ToStringInternal());
		}
	}
}
