using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public class ComplianceRuleIdParameter : ADIdParameter
	{
		public ComplianceRuleIdParameter()
		{
		}

		public ComplianceRuleIdParameter(Guid identity) : base(identity.ToString())
		{
		}

		public ComplianceRuleIdParameter(string identity) : base(Utils.ConvertObjectIdentityInFfo(identity))
		{
		}

		public ComplianceRuleIdParameter(ADObjectId adObjectId) : this(adObjectId.ToString())
		{
		}

		public ComplianceRuleIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public ComplianceRuleIdParameter(PsComplianceRuleBase rule) : this(rule.Identity.ToString())
		{
		}

		public static explicit operator string(ComplianceRuleIdParameter ruleIdParameter)
		{
			return ruleIdParameter.ToString();
		}

		public static ComplianceRuleIdParameter Parse(string identity)
		{
			return new ComplianceRuleIdParameter(identity);
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
