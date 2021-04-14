using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionBounceData : RuleActionData
	{
		public RuleActionBounceData()
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public uint Code { get; set; }

		public RuleActionBounceData(RuleAction.Bounce ruleAction) : base(ruleAction)
		{
			this.Code = (uint)ruleAction.Code;
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.Bounce((RuleAction.Bounce.BounceCode)this.Code);
		}

		protected override string ToStringInternal()
		{
			return string.Format("BOUNCE Code:{0}", this.Code);
		}
	}
}
