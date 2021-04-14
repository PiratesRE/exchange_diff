using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnifiedPolicyRuleClassification : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(string.Format("{0}\\{1}\\{2}", this.ObjectId, this.RuleId, this.ClassificationId));
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[UnifiedPolicyRuleClassificationSchema.OrganizationalUnitRootProperty];
			}
		}

		public Guid ObjectId
		{
			get
			{
				return (Guid)this[UnifiedPolicyRuleClassificationSchema.ObjectIdProperty];
			}
		}

		public string DataSource
		{
			get
			{
				return this[UnifiedPolicyRuleClassificationSchema.DataSourceProperty] as string;
			}
		}

		public Guid RuleId
		{
			get
			{
				return (Guid)this[UnifiedPolicyRuleClassificationSchema.RuleIdProperty];
			}
			set
			{
				this[UnifiedPolicyRuleClassificationSchema.RuleIdProperty] = value;
			}
		}

		public Guid ClassificationId
		{
			get
			{
				return (Guid)this[UnifiedPolicyRuleClassificationSchema.ClassificationIdProperty];
			}
			set
			{
				this[UnifiedPolicyRuleClassificationSchema.ClassificationIdProperty] = value;
			}
		}

		public int Count
		{
			get
			{
				return (int)this[UnifiedPolicyRuleClassificationSchema.CountProperty];
			}
			set
			{
				this[UnifiedPolicyRuleClassificationSchema.CountProperty] = value;
			}
		}

		public int Confidence
		{
			get
			{
				return (int)this[UnifiedPolicyRuleClassificationSchema.ConfidenceProperty];
			}
			set
			{
				this[UnifiedPolicyRuleClassificationSchema.ConfidenceProperty] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(UnifiedPolicyRuleClassificationSchema);
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			UnifiedPolicyRuleClassificationSchema.OrganizationalUnitRootProperty,
			UnifiedPolicyRuleClassificationSchema.ObjectIdProperty,
			UnifiedPolicyRuleClassificationSchema.DataSourceProperty,
			UnifiedPolicyRuleClassificationSchema.RuleIdProperty,
			UnifiedPolicyRuleClassificationSchema.ClassificationIdProperty,
			UnifiedPolicyRuleClassificationSchema.CountProperty,
			UnifiedPolicyRuleClassificationSchema.ConfidenceProperty,
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
