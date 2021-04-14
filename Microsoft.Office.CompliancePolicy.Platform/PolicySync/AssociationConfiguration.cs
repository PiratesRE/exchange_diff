using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	public sealed class AssociationConfiguration : PolicyConfigurationBase
	{
		public AssociationConfiguration() : base(ConfigurationObjectType.Association)
		{
			this.PolicyIds = Enumerable.Empty<Guid>();
		}

		public AssociationConfiguration(Guid tenantId, Guid objectId) : base(ConfigurationObjectType.Association, tenantId, objectId)
		{
			this.PolicyIds = Enumerable.Empty<Guid>();
		}

		[DataMember]
		public IncrementalAttribute<string> Comments { get; set; }

		[DataMember]
		public IncrementalAttribute<string> Description { get; set; }

		[DataMember]
		public IncrementalAttribute<AssociationType> AssociationType { get; set; }

		[DataMember]
		public IncrementalAttribute<string> Scope { get; set; }

		[DataMember]
		public IEnumerable<Guid> PolicyIds { get; set; }

		[DataMember]
		public IncrementalAttribute<Guid?> DefaultPolicyId { get; set; }

		[DataMember]
		public IncrementalAttribute<bool> AllowOverride { get; set; }

		protected override IDictionary<string, string> PropertyNameMapping
		{
			get
			{
				return AssociationConfiguration.propertyNameMapping;
			}
		}

		private static IDictionary<string, string> propertyNameMapping = new Dictionary<string, string>
		{
			{
				"Description",
				PolicyAssociationConfigSchema.Description
			},
			{
				"Comments",
				PolicyAssociationConfigSchema.Comment
			},
			{
				"PolicyIds",
				PolicyAssociationConfigSchema.PolicyDefinitionConfigIds
			},
			{
				"DefaultPolicyId",
				PolicyAssociationConfigSchema.DefaultPolicyDefinitionConfigId
			},
			{
				"AllowOverride",
				PolicyAssociationConfigSchema.AllowOverride
			},
			{
				"Scope",
				PolicyAssociationConfigSchema.Scope
			}
		}.Merge(PolicyConfigurationBase.BasePropertyNameMapping);
	}
}
