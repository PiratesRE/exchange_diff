using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnifiedPolicyRule : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(string.Format("{0}\\{1}", this.ObjectId, this.RuleId));
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[UnifiedPolicyRuleSchema.OrganizationalUnitRootProperty];
			}
		}

		public Guid ObjectId
		{
			get
			{
				return (Guid)this[UnifiedPolicyRuleSchema.ObjectIdProperty];
			}
		}

		public string DataSource
		{
			get
			{
				return this[UnifiedPolicyRuleSchema.DataSourceProperty] as string;
			}
		}

		public Guid RuleId
		{
			get
			{
				return (Guid)this[UnifiedPolicyRuleSchema.RuleIdProperty];
			}
			set
			{
				this[UnifiedPolicyRuleSchema.RuleIdProperty] = value;
			}
		}

		public Guid? DLPId
		{
			get
			{
				return new Guid?((Guid)this[UnifiedPolicyRuleSchema.DLPIdProperty]);
			}
			set
			{
				this[UnifiedPolicyRuleSchema.DLPIdProperty] = value;
			}
		}

		public string Mode
		{
			get
			{
				return this[UnifiedPolicyRuleSchema.ModeProperty] as string;
			}
			set
			{
				this[UnifiedPolicyRuleSchema.ModeProperty] = value;
			}
		}

		public string Severity
		{
			get
			{
				return this[UnifiedPolicyRuleSchema.SeverityProperty] as string;
			}
			set
			{
				this[UnifiedPolicyRuleSchema.SeverityProperty] = value;
			}
		}

		public string OverrideType
		{
			get
			{
				return this[UnifiedPolicyRuleSchema.OverrideTypeProperty] as string;
			}
			set
			{
				this[UnifiedPolicyRuleSchema.OverrideTypeProperty] = value;
			}
		}

		public string OverrideJustification
		{
			get
			{
				return this[UnifiedPolicyRuleSchema.OverrideJustificationProperty] as string;
			}
			set
			{
				this[UnifiedPolicyRuleSchema.OverrideJustificationProperty] = value;
			}
		}

		public List<UnifiedPolicyRuleAction> Actions
		{
			get
			{
				List<UnifiedPolicyRuleAction> result;
				if ((result = this.actions) == null)
				{
					result = (this.actions = new List<UnifiedPolicyRuleAction>());
				}
				return result;
			}
		}

		public List<UnifiedPolicyRuleClassification> Classifications
		{
			get
			{
				List<UnifiedPolicyRuleClassification> result;
				if ((result = this.classifications) == null)
				{
					result = (this.classifications = new List<UnifiedPolicyRuleClassification>());
				}
				return result;
			}
		}

		public void Add(UnifiedPolicyRuleAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			action[UnifiedPolicyRuleSchema.OrganizationalUnitRootProperty] = this.OrganizationalUnitRoot;
			action[UnifiedPolicyRuleSchema.ObjectIdProperty] = this.ObjectId;
			action[UnifiedPolicyRuleSchema.DataSourceProperty] = this.DataSource;
			action[UnifiedPolicyRuleSchema.RuleIdProperty] = this.RuleId;
			this.Actions.Add(action);
		}

		public void Add(UnifiedPolicyRuleClassification classification)
		{
			if (classification == null)
			{
				throw new ArgumentNullException("classification");
			}
			classification[UnifiedPolicyRuleClassificationSchema.OrganizationalUnitRootProperty] = this.OrganizationalUnitRoot;
			classification[UnifiedPolicyRuleClassificationSchema.ObjectIdProperty] = this.ObjectId;
			classification[UnifiedPolicyRuleClassificationSchema.DataSourceProperty] = this.DataSource;
			classification[UnifiedPolicyRuleClassificationSchema.RuleIdProperty] = this.RuleId;
			this.Classifications.Add(classification);
		}

		public override Type GetSchemaType()
		{
			return typeof(UnifiedPolicyRuleSchema);
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			UnifiedPolicyRuleSchema.OrganizationalUnitRootProperty,
			UnifiedPolicyRuleSchema.ObjectIdProperty,
			UnifiedPolicyRuleSchema.DataSourceProperty,
			UnifiedPolicyRuleSchema.RuleIdProperty,
			UnifiedPolicyRuleSchema.DLPIdProperty,
			UnifiedPolicyRuleSchema.ModeProperty,
			UnifiedPolicyRuleSchema.SeverityProperty,
			UnifiedPolicyRuleSchema.OverrideTypeProperty,
			UnifiedPolicyRuleSchema.OverrideJustificationProperty,
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

		private List<UnifiedPolicyRuleAction> actions;

		private List<UnifiedPolicyRuleClassification> classifications;
	}
}
