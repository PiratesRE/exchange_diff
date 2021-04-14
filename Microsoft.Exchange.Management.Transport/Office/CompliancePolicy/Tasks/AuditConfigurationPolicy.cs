using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public class AuditConfigurationPolicy : PsCompliancePolicyBase
	{
		public AuditConfigurationPolicy()
		{
		}

		public AuditConfigurationPolicy(PolicyStorage policyStorage) : base(policyStorage)
		{
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return AuditConfigurationPolicy.schema;
			}
		}

		public PolicyScenario Type
		{
			get
			{
				return PolicyScenario.AuditSettings;
			}
		}

		private static readonly AuditConfigurationPolicySchema schema = ObjectSchema.GetInstance<AuditConfigurationPolicySchema>();
	}
}
