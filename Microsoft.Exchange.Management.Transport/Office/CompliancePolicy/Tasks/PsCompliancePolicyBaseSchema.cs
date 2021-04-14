using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal class PsCompliancePolicyBaseSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<PolicyStorageSchema>();
		}

		public static readonly ADPropertyDefinition MasterIdentity = UnifiedPolicyStorageBaseSchema.MasterIdentity;

		public static readonly ADPropertyDefinition Comment = PolicyStorageSchema.Comments;

		public static readonly ADPropertyDefinition Enabled = PolicyStorageSchema.IsEnabled;

		public static readonly ADPropertyDefinition Mode = PolicyStorageSchema.EnforcementMode;

		public static readonly ADPropertyDefinition Workload = UnifiedPolicyStorageBaseSchema.WorkloadProp;

		public static readonly ADPropertyDefinition ObjectVersion = UnifiedPolicyStorageBaseSchema.PolicyVersion;
	}
}
