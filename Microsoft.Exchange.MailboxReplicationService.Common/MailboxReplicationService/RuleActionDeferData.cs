using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionDeferData : RuleActionData
	{
		public RuleActionDeferData()
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public byte[] Data { get; set; }

		public RuleActionDeferData(RuleAction.Defer ruleAction) : base(ruleAction)
		{
			this.Data = ruleAction.Data;
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.Defer(this.Data);
		}

		protected override string ToStringInternal()
		{
			return string.Format("DEFER {0}", TraceUtils.DumpBytes(this.Data));
		}
	}
}
