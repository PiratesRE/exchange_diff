using System;
using System.Runtime.Serialization;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	public sealed class ScopeConfiguration : PolicyConfigurationBase
	{
		public ScopeConfiguration() : base(ConfigurationObjectType.Scope)
		{
		}

		public ScopeConfiguration(Guid tenantId, Guid objectId) : base(ConfigurationObjectType.Scope, tenantId, objectId)
		{
		}

		[DataMember]
		public string AppliedScope { get; set; }

		[DataMember]
		public IncrementalAttribute<Mode> Mode { get; set; }
	}
}
