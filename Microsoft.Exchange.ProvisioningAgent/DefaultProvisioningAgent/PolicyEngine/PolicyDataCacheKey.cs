using System;
using System.ComponentModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Provisioning;

namespace Microsoft.Exchange.DefaultProvisioningAgent.PolicyEngine
{
	[ImmutableObject(true)]
	internal class PolicyDataCacheKey : IEquatable<PolicyDataCacheKey>
	{
		public PolicyDataCacheKey(OrganizationId organizationId, Type poType, ProvisioningPolicyType policyType)
		{
			if (null == organizationId)
			{
				throw new ArgumentNullException("organizationId");
			}
			if (null == poType)
			{
				throw new ArgumentNullException("poType");
			}
			if ((policyType & ~(ProvisioningPolicyType.Template | ProvisioningPolicyType.Enforcement)) != (ProvisioningPolicyType)0)
			{
				throw new ArgumentOutOfRangeException("policyType");
			}
			if (!PolicyConfiguration.ObjectType2PolicyEntryDictionary.ContainsKey(poType))
			{
				throw new ArgumentOutOfRangeException("poType");
			}
			this.organizationId = organizationId;
			this.poType = poType;
			this.policyType = policyType;
		}

		public Type ObjectType
		{
			get
			{
				return this.poType;
			}
		}

		public ProvisioningPolicyType PolicyType
		{
			get
			{
				return this.policyType;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		public bool Equals(PolicyDataCacheKey other)
		{
			return other != null && (this.ObjectType.Equals(other.ObjectType) && this.PolicyType.Equals(other.PolicyType)) && object.Equals(this.OrganizationId, other.OrganizationId);
		}

		public override bool Equals(object other)
		{
			return other is PolicyDataCacheKey && this.Equals((PolicyDataCacheKey)other);
		}

		public override int GetHashCode()
		{
			return this.ObjectType.GetHashCode() ^ this.PolicyType.GetHashCode() ^ this.OrganizationId.GetHashCode();
		}

		private OrganizationId organizationId;

		private Type poType;

		private ProvisioningPolicyType policyType;
	}
}
