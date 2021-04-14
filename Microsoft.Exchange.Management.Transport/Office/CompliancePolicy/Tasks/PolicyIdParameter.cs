using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public class PolicyIdParameter : ADIdParameter
	{
		public PolicyIdParameter()
		{
		}

		public PolicyIdParameter(Guid identity) : base(identity.ToString())
		{
		}

		public PolicyIdParameter(string identity) : base(Utils.ConvertObjectIdentityInFfo(identity))
		{
		}

		public PolicyIdParameter(ADObjectId adObjectId) : this(adObjectId.ToString())
		{
		}

		public PolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public PolicyIdParameter(PsCompliancePolicyBase policy) : this(policy.Identity.ToString())
		{
		}

		public static explicit operator string(PolicyIdParameter policyIdParameter)
		{
			return policyIdParameter.ToString();
		}

		public static PolicyIdParameter Parse(string identity)
		{
			return new PolicyIdParameter(identity);
		}

		internal override ADPropertyDefinition[] AdditionalMatchingProperties
		{
			get
			{
				return new ADPropertyDefinition[]
				{
					UnifiedPolicyStorageBaseSchema.MasterIdentity
				};
			}
		}
	}
}
