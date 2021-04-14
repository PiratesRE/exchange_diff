using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.Supervision
{
	[Cmdlet("Get", "SupervisionPolicy")]
	public sealed class GetSupervisionPolicy : GetMultitenancySingletonSystemConfigurationObjectTask<TransportRule>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter DisplayDetails
		{
			get
			{
				return (SwitchParameter)(base.Fields["DisplayDetails"] ?? false);
			}
			set
			{
				base.Fields["DisplayDetails"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			Dictionary<string, string> ruleNames = GetSupervisionPolicy.GetRuleNames();
			ADRuleStorageManager adruleStorageManager = null;
			try
			{
				adruleStorageManager = new ADRuleStorageManager("TransportVersioned", base.DataSession);
			}
			catch (RuleCollectionNotInAdException exception)
			{
				base.WriteError(exception, (ErrorCategory)1003, null);
			}
			QueryFilter queryFilter = GetSupervisionPolicy.GetQueryFilter(ruleNames.Keys);
			adruleStorageManager.LoadRuleCollectionWithoutParsing(queryFilter);
			try
			{
				adruleStorageManager.ParseRuleCollection();
			}
			catch (ParserException exception2)
			{
				base.WriteError(exception2, (ErrorCategory)1003, null);
			}
			SupervisionPolicy presentationObject = GetSupervisionPolicy.GetPresentationObject(adruleStorageManager, ref ruleNames, this.DisplayDetails.ToBool());
			this.WriteResult(presentationObject);
			string missingPolicies = GetSupervisionPolicy.GetMissingPolicies(ruleNames);
			if (!string.IsNullOrEmpty(missingPolicies))
			{
				this.WriteWarning(Strings.SupervisionPoliciesNotFound(missingPolicies));
			}
			TaskLogger.LogExit();
		}

		private static SupervisionPolicy GetPresentationObject(ADRuleStorageManager storageManager, ref Dictionary<string, string> rules, bool displayDetails)
		{
			SupervisionPolicy supervisionPolicy = new SupervisionPolicy("SupervisionPolicy" + storageManager.RuleCollectionId.GetHashCode().ToString());
			foreach (Microsoft.Exchange.MessagingPolicies.Rules.Rule rule in storageManager.GetRuleCollection())
			{
				TransportRule transportRule = (TransportRule)rule;
				if (!transportRule.IsTooAdvancedToParse)
				{
					Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Rule rule2 = Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Rule.CreateFromInternalRule(TransportRulePredicate.BridgeheadMappings, TransportRuleAction.BridgeheadMappings, transportRule, 0, null);
					if (transportRule.Name.Equals(GetSupervisionPolicy.ClosedCampusInboundRuleName))
					{
						supervisionPolicy.ClosedCampusInboundPolicyEnabled = (transportRule.Enabled == RuleState.Enabled);
						if (displayDetails)
						{
							supervisionPolicy.ClosedCampusInboundPolicyGroupExceptions = GetSupervisionPolicy.ConvertToSmtpAddressMVP(rule2.ExceptIfSentToMemberOf);
							supervisionPolicy.ClosedCampusInboundPolicyDomainExceptions = GetSupervisionPolicy.ConvertToSmtpDomains(rule2.ExceptIfFromAddressMatchesPatterns);
						}
						rules.Remove(GetSupervisionPolicy.ClosedCampusInboundRuleName);
					}
					else if (transportRule.Name.Equals(GetSupervisionPolicy.ClosedCampusOutboundRuleName))
					{
						supervisionPolicy.ClosedCampusOutboundPolicyEnabled = (transportRule.Enabled == RuleState.Enabled);
						if (displayDetails)
						{
							supervisionPolicy.ClosedCampusOutboundPolicyGroupExceptions = GetSupervisionPolicy.ConvertToSmtpAddressMVP(rule2.ExceptIfFromMemberOf);
							supervisionPolicy.ClosedCampusOutboundPolicyDomainExceptions = GetSupervisionPolicy.ConvertToSmtpDomains(rule2.ExceptIfRecipientAddressMatchesPatterns);
						}
						rules.Remove(GetSupervisionPolicy.ClosedCampusOutboundRuleName);
					}
					else if (transportRule.Name.Equals(GetSupervisionPolicy.BadWordsRuleName))
					{
						supervisionPolicy.BadWordsPolicyEnabled = (transportRule.Enabled == RuleState.Enabled);
						if (displayDetails)
						{
							supervisionPolicy.BadWordsList = GetSupervisionPolicy.ConvertToCommaSeparatedString(rule2.SubjectOrBodyContainsWords);
						}
						rules.Remove(GetSupervisionPolicy.BadWordsRuleName);
					}
					else if (transportRule.Name.Equals(GetSupervisionPolicy.AntiBullyingRuleName))
					{
						supervisionPolicy.AntiBullyingPolicyEnabled = (transportRule.Enabled == RuleState.Enabled);
						rules.Remove(GetSupervisionPolicy.AntiBullyingRuleName);
					}
				}
			}
			return supervisionPolicy;
		}

		private static QueryFilter GetQueryFilter(IEnumerable<string> ruleNames)
		{
			QueryFilter queryFilter = null;
			foreach (string propertyValue in ruleNames)
			{
				QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, propertyValue);
				if (queryFilter != null)
				{
					queryFilter = new OrFilter(new QueryFilter[]
					{
						queryFilter,
						queryFilter2
					});
				}
				else
				{
					queryFilter = queryFilter2;
				}
			}
			return queryFilter;
		}

		internal static Dictionary<string, string> GetRuleNames()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(4);
			dictionary[GetSupervisionPolicy.ClosedCampusInboundRuleName] = "Closed campus inbound";
			dictionary[GetSupervisionPolicy.ClosedCampusOutboundRuleName] = "Closed campus outbound";
			dictionary[GetSupervisionPolicy.BadWordsRuleName] = "Bad words";
			dictionary[GetSupervisionPolicy.AntiBullyingRuleName] = "Anti bullying";
			return dictionary;
		}

		private static string GetMissingPolicies(Dictionary<string, string> missingRules)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in missingRules)
			{
				if (!string.IsNullOrEmpty(stringBuilder.ToString()))
				{
					stringBuilder.Append(", ");
					stringBuilder.Append(keyValuePair.Value);
				}
				else
				{
					stringBuilder.Append(keyValuePair.Value);
				}
			}
			return stringBuilder.ToString();
		}

		private static SmtpDomain[] ConvertToSmtpDomains(Pattern[] patterns)
		{
			if (patterns == null)
			{
				return null;
			}
			SmtpDomain[] array = new SmtpDomain[patterns.Length];
			int num = 0;
			foreach (Pattern pattern in patterns)
			{
				string text = pattern.ToString();
				array[num++] = new SmtpDomain(text.Substring(1, text.Length - 1));
			}
			return array;
		}

		private static MultiValuedProperty<SmtpAddress> ConvertToSmtpAddressMVP(RecipientIdParameter[] idParameters)
		{
			MultiValuedProperty<SmtpAddress> multiValuedProperty = new MultiValuedProperty<SmtpAddress>();
			if (idParameters == null)
			{
				return multiValuedProperty;
			}
			foreach (RecipientIdParameter recipientIdParameter in idParameters)
			{
				multiValuedProperty.Add(SmtpAddress.Parse(recipientIdParameter.RawIdentity));
			}
			return multiValuedProperty;
		}

		private static string ConvertToCommaSeparatedString(Word[] words)
		{
			if (words == null || words.Length == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(words[0].Value);
			for (int i = 1; i < words.Length; i++)
			{
				stringBuilder.Append(SupervisionPolicy.BadWordsSeparator);
				stringBuilder.Append(" ");
				stringBuilder.Append(words[i].Value);
			}
			return stringBuilder.ToString();
		}

		internal const string RuleCollectionName = "TransportVersioned";

		internal const string RuleNamePrefix = "__";

		internal const string ClosedCampusInboundPresentationName = "Closed campus inbound";

		internal const string ClosedCampusOutboundPresentationName = "Closed campus outbound";

		internal const string BadWordsPresentationName = "Bad words";

		internal const string AntiBullyingPresentationName = "Anti bullying";

		private const int supervisionRuleCount = 4;

		public const string ClosedCampusInboundRejectReasonText = "You can't send e-mail to people in this organization.";

		public const string ClosedCampusOutboundRejectReasonText = "You can't send e-mail to people outside this organization.";

		public const string BadWordsRejectReasonText = "This message contains inappropriate language that's not permitted by the organization.";

		public const string AntiBullyingRejectReasonText = "You're not allowed to send e-mail to this person";

		public static readonly string ClosedCampusInboundRuleName = "__" + "closedcampusinbound";

		public static readonly string ClosedCampusOutboundRuleName = "__" + "closedcampusoutbound";

		public static readonly string BadWordsRuleName = "__" + "badwords";

		public static readonly string AntiBullyingRuleName = "__" + "antibullying";
	}
}
