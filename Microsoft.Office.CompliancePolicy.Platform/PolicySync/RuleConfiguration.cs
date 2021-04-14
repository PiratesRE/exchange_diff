using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	public sealed class RuleConfiguration : PolicyConfigurationBase
	{
		public RuleConfiguration() : base(ConfigurationObjectType.Rule)
		{
		}

		public RuleConfiguration(Guid tenantId, Guid objectId) : base(ConfigurationObjectType.Rule, tenantId, objectId)
		{
		}

		[DataMember]
		public IncrementalAttribute<string> Comments { get; set; }

		[DataMember]
		public IncrementalAttribute<string> Description { get; set; }

		[DataMember]
		public Guid ParentPolicyId { get; set; }

		[DataMember]
		public IncrementalAttribute<Mode> Mode { get; set; }

		[DataMember]
		public IncrementalAttribute<string> RuleBlob { get; set; }

		[DataMember]
		public IncrementalAttribute<int> Priority { get; set; }

		[DataMember]
		public IncrementalAttribute<bool> IsEnabled { get; set; }

		[DataMember]
		public IncrementalAttribute<string> CreatedBy { get; set; }

		[DataMember]
		public string LastModifiedBy { get; set; }

		[DataMember]
		public PolicyScenario PolicyScenario { get; set; }

		protected override IDictionary<string, string> PropertyNameMapping
		{
			get
			{
				return RuleConfiguration.propertyNameMapping;
			}
		}

		private static IDictionary<string, string> propertyNameMapping = new Dictionary<string, string>
		{
			{
				"Mode",
				PolicyRuleConfigSchema.Mode
			},
			{
				"RuleBlob",
				PolicyRuleConfigSchema.RuleBlob
			},
			{
				"Priority",
				PolicyRuleConfigSchema.Priority
			},
			{
				"Description",
				PolicyRuleConfigSchema.Description
			},
			{
				"Comments",
				PolicyRuleConfigSchema.Comment
			},
			{
				"ParentPolicyId",
				PolicyRuleConfigSchema.PolicyDefinitionConfigId
			},
			{
				"IsEnabled",
				PolicyRuleConfigSchema.Enabled
			},
			{
				"CreatedBy",
				PolicyRuleConfigSchema.CreatedBy
			},
			{
				"LastModifiedBy",
				PolicyRuleConfigSchema.LastModifiedBy
			},
			{
				"PolicyScenario",
				PolicyRuleConfigSchema.Scenario
			}
		}.Merge(PolicyConfigurationBase.BasePropertyNameMapping);
	}
}
