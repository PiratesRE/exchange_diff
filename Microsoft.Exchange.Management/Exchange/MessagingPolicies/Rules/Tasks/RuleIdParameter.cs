using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class RuleIdParameter : ADIdParameter
	{
		public RuleIdParameter()
		{
		}

		public RuleIdParameter(string identity) : base(identity)
		{
		}

		public RuleIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RuleIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static explicit operator string(RuleIdParameter ruleIdParameter)
		{
			return ruleIdParameter.ToString();
		}

		public static RuleIdParameter Parse(string identity)
		{
			return new RuleIdParameter(identity);
		}

		public static ADObjectId GetRuleCollectionRdn(string ruleCollection)
		{
			ADObjectId adobjectId = new ADObjectId(new AdName("cn", "Transport Settings"));
			return adobjectId.GetChildId("Rules").GetChildId(ruleCollection);
		}

		public static ADObjectId GetRuleCollectionId(IConfigDataProvider session, string ruleCollection)
		{
			IConfigurationSession configurationSession = (IConfigurationSession)session;
			ADObjectId orgContainerId = configurationSession.GetOrgContainerId();
			return orgContainerId.GetChildId("Transport Settings").GetChildId("Rules").GetChildId(ruleCollection);
		}

		internal override ADPropertyDefinition[] AdditionalMatchingProperties
		{
			get
			{
				return new ADPropertyDefinition[]
				{
					TransportRuleSchema.ImmutableId
				};
			}
		}
	}
}
