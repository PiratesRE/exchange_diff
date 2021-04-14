using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Directory.UnifiedPolicy
{
	[Serializable]
	public abstract class UnifiedPolicyStorageBase : ADConfigurationObject
	{
		public UnifiedPolicyStorageBase()
		{
		}

		public Workload Workload
		{
			get
			{
				return (Workload)this[UnifiedPolicyStorageBaseSchema.WorkloadProp];
			}
			set
			{
				this[UnifiedPolicyStorageBaseSchema.WorkloadProp] = value;
			}
		}

		public Guid PolicyVersion
		{
			get
			{
				return (Guid)this[UnifiedPolicyStorageBaseSchema.PolicyVersion];
			}
			set
			{
				this[UnifiedPolicyStorageBaseSchema.PolicyVersion] = value;
			}
		}

		public Guid MasterIdentity
		{
			get
			{
				return (Guid)this[UnifiedPolicyStorageBaseSchema.MasterIdentity];
			}
			set
			{
				this[UnifiedPolicyStorageBaseSchema.MasterIdentity] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}
	}
}
