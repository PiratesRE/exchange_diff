using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[Serializable]
	public class DlpPolicyIdParameter : ADIdParameter
	{
		public DlpPolicyIdParameter()
		{
		}

		public DlpPolicyIdParameter(string identity) : base(identity)
		{
		}

		public DlpPolicyIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public DlpPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static explicit operator string(DlpPolicyIdParameter dlpPolicyIdParameter)
		{
			return dlpPolicyIdParameter.ToString();
		}

		public static DlpPolicyIdParameter Parse(string identity)
		{
			return new DlpPolicyIdParameter(identity);
		}

		internal override ADPropertyDefinition[] AdditionalMatchingProperties
		{
			get
			{
				return new ADPropertyDefinition[]
				{
					ADComplianceProgramSchema.ImmutableId
				};
			}
		}

		public static ADObjectId GetDlpPolicyCollectionRdn()
		{
			ADObjectId adobjectId = new ADObjectId(new AdName("cn", "Transport Settings"));
			return adobjectId.GetChildId("Rules").GetChildId(DlpUtils.TenantDlpPoliciesCollectionName);
		}
	}
}
