using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RuleActionTagData : RuleActionData
	{
		public RuleActionTagData()
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public PropValueData Value { get; set; }

		public RuleActionTagData(RuleAction.Tag ruleAction) : base(ruleAction)
		{
			this.Value = DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(ruleAction.Value);
		}

		protected override RuleAction GetRuleActionInternal()
		{
			return new RuleAction.Tag(DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(this.Value));
		}

		protected override void EnumPropTagsInternal(CommonUtils.EnumPropTagDelegate del)
		{
			int propTag = this.Value.PropTag;
			del(ref propTag);
			this.Value.PropTag = propTag;
		}

		protected override void EnumPropValuesInternal(CommonUtils.EnumPropValueDelegate del)
		{
			del(this.Value);
		}

		protected override string ToStringInternal()
		{
			return string.Format("TAG {0}", this.Value);
		}
	}
}
