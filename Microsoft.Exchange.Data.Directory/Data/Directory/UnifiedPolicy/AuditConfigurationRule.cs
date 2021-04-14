using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Exchange.Data.Directory.UnifiedPolicy
{
	[Serializable]
	public class AuditConfigurationRule : ADPresentationObject
	{
		public AuditConfigurationRule()
		{
		}

		public AuditConfigurationRule(RuleStorage ruleStorage) : base(ruleStorage)
		{
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return AuditConfigurationRule.schema;
			}
		}

		internal Guid MasterIdentity
		{
			get
			{
				return (Guid)this[AuditConfigurationRuleSchema.MasterIdentity];
			}
			set
			{
				this[AuditConfigurationRuleSchema.MasterIdentity] = value;
			}
		}

		internal string RuleBlob
		{
			get
			{
				return (string)this[AuditConfigurationRuleSchema.RuleBlob];
			}
			set
			{
				this[AuditConfigurationRuleSchema.RuleBlob] = value;
			}
		}

		public Workload Workload
		{
			get
			{
				return (Workload)this[AuditConfigurationRuleSchema.Workload];
			}
			set
			{
				this[AuditConfigurationRuleSchema.Workload] = value;
			}
		}

		public MultiValuedProperty<AuditableOperations> AuditOperation { get; set; }

		public Guid Policy
		{
			get
			{
				return (Guid)this[AuditConfigurationRuleSchema.Policy];
			}
			set
			{
				this[AuditConfigurationRuleSchema.Policy] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal void PopulateTaskProperties()
		{
			if (!string.IsNullOrEmpty(this.RuleBlob))
			{
				this.SetTaskCondition(this.GetPolicyRuleFromRuleBlob().Condition as AuditOperationsPredicate);
			}
		}

		internal void UpdateStorageProperties()
		{
			PolicyRule policyRule = new PolicyRule
			{
				Condition = new AuditOperationsPredicate(this.GetTaskConditions()),
				Actions = AuditConfigurationRule.EmptyActionList,
				Comments = string.Empty,
				Enabled = RuleState.Disabled,
				ImmutableId = base.Guid,
				Name = base.Name
			};
			this.RuleBlob = this.GetRuleXmlFromPolicyRule(policyRule);
		}

		internal PolicyRule GetPolicyRuleFromRuleBlob()
		{
			return new RuleParser(new SimplePolicyParserFactory()).GetRule(this.RuleBlob);
		}

		internal string GetRuleXmlFromPolicyRule(PolicyRule policyRule)
		{
			return new RuleSerializer().SaveRuleToString(policyRule);
		}

		private List<string> GetTaskConditions()
		{
			List<string> list = new List<string>();
			if (this.AuditOperation != null)
			{
				foreach (AuditableOperations auditableOperations in this.AuditOperation)
				{
					list.Add(auditableOperations.ToString());
				}
			}
			if (!list.Any<string>())
			{
				list.Add(AuditableOperations.None.ToString());
			}
			return list;
		}

		private void SetTaskCondition(AuditOperationsPredicate condition)
		{
			MultiValuedProperty<AuditableOperations> multiValuedProperty = new MultiValuedProperty<AuditableOperations>();
			if (condition != null && condition.Value != null && condition.Value.ParsedValue != null)
			{
				if (condition.Value.ParsedValue is string)
				{
					AuditableOperations item;
					if (Enum.TryParse<AuditableOperations>((string)condition.Value.ParsedValue, true, out item))
					{
						multiValuedProperty.Add(item);
					}
				}
				else if (condition.Value.ParsedValue is List<string>)
				{
					foreach (string text in ((List<string>)condition.Value.ParsedValue))
					{
						AuditableOperations item;
						if (text != null && Enum.TryParse<AuditableOperations>(text, true, out item))
						{
							multiValuedProperty.Add(item);
						}
					}
				}
			}
			this.AuditOperation = multiValuedProperty;
		}

		private static AuditConfigurationRuleSchema schema = ObjectSchema.GetInstance<AuditConfigurationRuleSchema>();

		private static List<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action> EmptyActionList = new List<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action>();
	}
}
