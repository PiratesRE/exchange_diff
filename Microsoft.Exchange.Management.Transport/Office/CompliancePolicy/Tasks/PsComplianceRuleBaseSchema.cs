using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal class PsComplianceRuleBaseSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<RuleStorageSchema>();
		}

		public static readonly ADPropertyDefinition MasterIdentity = UnifiedPolicyStorageBaseSchema.MasterIdentity;

		public static readonly ADPropertyDefinition RuleBlob = RuleStorageSchema.RuleBlob;

		public static readonly ADPropertyDefinition Workload = UnifiedPolicyStorageBaseSchema.WorkloadProp;

		public static readonly ADPropertyDefinition Policy = RuleStorageSchema.ParentPolicyId;

		public static readonly ADPropertyDefinition Comment = RuleStorageSchema.Comments;

		public static readonly ADPropertyDefinition Enabled = RuleStorageSchema.IsEnabled;

		public static readonly ADPropertyDefinition Mode = RuleStorageSchema.EnforcementMode;

		public static readonly ADPropertyDefinition ObjectVersion = UnifiedPolicyStorageBaseSchema.PolicyVersion;

		public static readonly ADPropertyDefinition ContentMatchQuery = RuleStorageSchema.ContentMatchQuery;
	}
}
