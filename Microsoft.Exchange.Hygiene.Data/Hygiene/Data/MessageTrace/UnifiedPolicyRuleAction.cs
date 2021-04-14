using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnifiedPolicyRuleAction : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(string.Format("{0}\\{1}\\{2}", this.ObjectId, this.RuleId, this.Action));
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[UnifiedPolicyRuleActionSchema.OrganizationalUnitRootProperty];
			}
		}

		public Guid ObjectId
		{
			get
			{
				return (Guid)this[UnifiedPolicyRuleActionSchema.ObjectIdProperty];
			}
		}

		public string DataSource
		{
			get
			{
				return this[UnifiedPolicyRuleActionSchema.DataSourceProperty] as string;
			}
		}

		public Guid RuleId
		{
			get
			{
				return (Guid)this[UnifiedPolicyRuleActionSchema.RuleIdProperty];
			}
		}

		public string Action
		{
			get
			{
				return this[UnifiedPolicyRuleActionSchema.ActionProperty] as string;
			}
			set
			{
				this[UnifiedPolicyRuleActionSchema.ActionProperty] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(UnifiedPolicyRuleActionSchema);
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			UnifiedPolicyRuleActionSchema.OrganizationalUnitRootProperty,
			UnifiedPolicyRuleActionSchema.ObjectIdProperty,
			UnifiedPolicyRuleActionSchema.DataSourceProperty,
			UnifiedPolicyRuleActionSchema.RuleIdProperty,
			UnifiedPolicyRuleActionSchema.ActionProperty,
			UnifiedPolicyCommonSchema.HashBucketProperty,
			UnifiedPolicyCommonSchema.IntValue1Prop,
			UnifiedPolicyCommonSchema.IntValue2Prop,
			UnifiedPolicyCommonSchema.IntValue3Prop,
			UnifiedPolicyCommonSchema.LongValue1Prop,
			UnifiedPolicyCommonSchema.LongValue2Prop,
			UnifiedPolicyCommonSchema.GuidValue1Prop,
			UnifiedPolicyCommonSchema.GuidValue2Prop,
			UnifiedPolicyCommonSchema.StringValue1Prop,
			UnifiedPolicyCommonSchema.StringValue2Prop,
			UnifiedPolicyCommonSchema.StringValue3Prop,
			UnifiedPolicyCommonSchema.StringValue4Prop,
			UnifiedPolicyCommonSchema.StringValue5Prop,
			UnifiedPolicyCommonSchema.ByteValue1Prop,
			UnifiedPolicyCommonSchema.ByteValue2Prop
		};
	}
}
