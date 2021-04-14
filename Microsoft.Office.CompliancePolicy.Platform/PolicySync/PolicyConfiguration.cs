using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	public sealed class PolicyConfiguration : PolicyConfigurationBase
	{
		public PolicyConfiguration() : base(ConfigurationObjectType.Policy)
		{
		}

		public PolicyConfiguration(Guid tenantId, Guid objectId) : base(ConfigurationObjectType.Policy, tenantId, objectId)
		{
		}

		[DataMember]
		public IncrementalAttribute<string> Comments { get; set; }

		[DataMember]
		public IncrementalAttribute<string> Description { get; set; }

		[DataMember]
		public IncrementalAttribute<Mode> Mode { get; set; }

		[DataMember]
		public PolicyScenario PolicyScenario { get; set; }

		[DataMember]
		public IncrementalAttribute<Guid?> DefaultRuleId { get; set; }

		[DataMember]
		public IncrementalAttribute<bool> IsEnabled { get; set; }

		[DataMember]
		public IncrementalAttribute<string> CreatedBy { get; set; }

		[DataMember]
		public string LastModifiedBy { get; set; }

		protected override IDictionary<string, string> PropertyNameMapping
		{
			get
			{
				return PolicyConfiguration.propertyNameMapping;
			}
		}

		private static IDictionary<string, string> propertyNameMapping = new Dictionary<string, string>
		{
			{
				"Description",
				PolicyDefinitionConfigSchema.Description
			},
			{
				"Comments",
				PolicyDefinitionConfigSchema.Comment
			},
			{
				"DefaultRuleId",
				PolicyDefinitionConfigSchema.DefaultPolicyRuleConfigId
			},
			{
				"Mode",
				PolicyDefinitionConfigSchema.Mode
			},
			{
				"PolicyScenario",
				PolicyDefinitionConfigSchema.Scenario
			},
			{
				"IsEnabled",
				PolicyDefinitionConfigSchema.Enabled
			},
			{
				"CreatedBy",
				PolicyDefinitionConfigSchema.CreatedBy
			},
			{
				"LastModifiedBy",
				PolicyDefinitionConfigSchema.LastModifiedBy
			}
		}.Merge(PolicyConfigurationBase.BasePropertyNameMapping);
	}
}
