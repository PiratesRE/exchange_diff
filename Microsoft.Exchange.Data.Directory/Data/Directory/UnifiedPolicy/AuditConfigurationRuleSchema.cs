using System;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Data.Directory.UnifiedPolicy
{
	internal class AuditConfigurationRuleSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<RuleStorageSchema>();
		}

		public static readonly ADPropertyDefinition MasterIdentity = UnifiedPolicyStorageBaseSchema.MasterIdentity;

		public static readonly ADPropertyDefinition RuleBlob = RuleStorageSchema.RuleBlob;

		public static readonly ADPropertyDefinition Workload = UnifiedPolicyStorageBaseSchema.WorkloadProp;

		public static readonly ADPropertyDefinition Policy = RuleStorageSchema.ParentPolicyId;
	}
}
