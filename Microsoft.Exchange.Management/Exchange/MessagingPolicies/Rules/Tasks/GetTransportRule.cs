using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks;
using Microsoft.Exchange.TextProcessing;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Cmdlet("Get", "TransportRule", DefaultParameterSetName = "Identity")]
	public sealed class GetTransportRule : GetMultitenancySystemConfigurationObjectTask<RuleIdParameter, TransportRule>
	{
		public GetTransportRule()
		{
			this.ruleCollectionName = Utils.RuleCollectionNameFromRole();
			this.supportedPredicates = TransportRulePredicate.GetAvailablePredicateMappings();
			this.supportedActions = TransportRuleAction.GetAvailableActionMappings();
			this.State = RuleState.Enabled;
			base.Fields.ResetChangeTracking();
		}

		[Parameter(Mandatory = false)]
		public RuleState State
		{
			get
			{
				return (RuleState)base.Fields["State"];
			}
			set
			{
				base.Fields["State"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DlpPolicy
		{
			get
			{
				return (string)base.Fields["DlpPolicy"];
			}
			set
			{
				base.Fields["DlpPolicy"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string Filter
		{
			get
			{
				return (string)base.Fields["Filter"];
			}
			set
			{
				base.Fields["Filter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.Identity != null)
				{
					return null;
				}
				return RuleIdParameter.GetRuleCollectionId(base.DataSession, this.ruleCollectionName);
			}
		}

		protected override void InternalValidate()
		{
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = RuleIdParameter.GetRuleCollectionRdn(this.ruleCollectionName);
			}
			if (base.Fields["Filter"] != null)
			{
				if (base.Fields["Identity"] != null)
				{
					base.WriteError(new ArgumentException(Strings.IncompatibleGetTransportRuleParameters("Identity", "Filter")), ErrorCategory.InvalidArgument, "Filter");
				}
				string errorMessage;
				this.searchFilter = GetTransportRule.ParseFilterParameter((string)base.Fields["Filter"], out errorMessage);
				if (this.searchFilter == null)
				{
					base.WriteError(new ArgumentException(Strings.InvalidRuleSearchFilter((string)base.Fields["Filter"], errorMessage)), ErrorCategory.InvalidArgument, "Filter");
				}
			}
			base.InternalValidate();
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			try
			{
				if (this.Identity == null)
				{
					ADRuleStorageManager adruleStorageManager = new ADRuleStorageManager(this.ruleCollectionName, base.DataSession);
					adruleStorageManager.LoadRuleCollectionWithoutParsing();
					for (int i = 0; i < adruleStorageManager.Count; i++)
					{
						TransportRule transportRule;
						adruleStorageManager.GetRuleWithoutParsing(i, out transportRule);
						this.OutputRule(i, transportRule);
					}
				}
				else
				{
					List<TransportRule> list = new List<TransportRule>();
					list.AddRange((IEnumerable<TransportRule>)dataObjects);
					Dictionary<OrganizationId, ADRuleStorageManager> ruleCollections = this.GetRuleCollections(list);
					foreach (KeyValuePair<OrganizationId, ADRuleStorageManager> keyValuePair in ruleCollections)
					{
						for (int j = 0; j < keyValuePair.Value.Count; j++)
						{
							TransportRule transportRule;
							keyValuePair.Value.GetRuleWithoutParsing(j, out transportRule);
							if (Utils.IsRuleIdInList(transportRule.Id, list))
							{
								this.OutputRule(j, transportRule);
							}
						}
					}
				}
			}
			catch (RuleCollectionNotInAdException)
			{
			}
		}

		private Dictionary<OrganizationId, ADRuleStorageManager> GetRuleCollections(IEnumerable<TransportRule> rules)
		{
			Dictionary<OrganizationId, ADRuleStorageManager> dictionary = new Dictionary<OrganizationId, ADRuleStorageManager>();
			foreach (TransportRule transportRule in rules)
			{
				if (!dictionary.ContainsKey(transportRule.OrganizationId))
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, transportRule.OrganizationId, base.ExecutingUserOrganizationId, false);
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 242, "GetRuleCollections", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\TransportRule\\GetTransportRule.cs");
					ADRuleStorageManager adruleStorageManager;
					try
					{
						adruleStorageManager = new ADRuleStorageManager(this.ruleCollectionName, tenantOrTopologyConfigurationSession);
						adruleStorageManager.LoadRuleCollectionWithoutParsing();
					}
					catch (RuleCollectionNotInAdException)
					{
						continue;
					}
					dictionary.Add(transportRule.OrganizationId, adruleStorageManager);
				}
			}
			return dictionary;
		}

		private void OutputRule(int priority, TransportRule transportRule)
		{
			TransportRule transportRule2 = null;
			ParserException ex = null;
			if (base.NeedSuppressingPiiData)
			{
				base.ExchangeRunspaceConfig.EnablePiiMap = true;
			}
			try
			{
				transportRule2 = (TransportRule)TransportRuleParser.Instance.GetRule(transportRule.Xml);
			}
			catch (ParserException ex2)
			{
				ex = ex2;
			}
			if (transportRule2 == null || (this.StateMatches(transportRule2) && this.DlpPolicyMatches(transportRule2)))
			{
				if (transportRule2 == null && base.Fields["Filter"] == null)
				{
					Rule rule = Rule.CreateCorruptRule(priority, transportRule, Strings.CorruptRule(transportRule.Name, (ex != null) ? ex.Message : string.Empty));
					if (base.NeedSuppressingPiiData)
					{
						rule.SuppressPiiData(Utils.GetSessionPiiMap(base.ExchangeRunspaceConfig));
					}
					this.WriteResult(rule);
					return;
				}
				if (transportRule2.IsTooAdvancedToParse && base.Fields["Filter"] == null)
				{
					this.WriteWarning(Strings.CannotParseRuleDueToVersion(transportRule.Name));
					Rule rule2 = Rule.CreateAdvancedRule(priority, transportRule, transportRule2.Enabled);
					if (base.NeedSuppressingPiiData)
					{
						rule2.SuppressPiiData(Utils.GetSessionPiiMap(base.ExchangeRunspaceConfig));
					}
					this.WriteResult(rule2);
					return;
				}
				try
				{
					Rule rule3 = Rule.CreateFromInternalRule(this.supportedPredicates, this.supportedActions, transportRule2, priority, transportRule);
					if (this.FilterMatches(rule3))
					{
						if (base.NeedSuppressingPiiData)
						{
							rule3.SuppressPiiData(Utils.GetSessionPiiMap(base.ExchangeRunspaceConfig));
						}
						this.WriteResult(rule3);
					}
				}
				catch (ArgumentException ex3)
				{
					Rule rule4 = Rule.CreateCorruptRule(priority, transportRule, Strings.CorruptRule(transportRule.Name, ex3.Message));
					if (base.NeedSuppressingPiiData)
					{
						rule4.SuppressPiiData(Utils.GetSessionPiiMap(base.ExchangeRunspaceConfig));
					}
					this.WriteResult(rule4);
				}
			}
		}

		private bool StateMatches(TransportRule rule)
		{
			return !base.Fields.IsModified("State") || this.State == rule.Enabled;
		}

		private bool DlpPolicyMatches(TransportRule rule)
		{
			if (!base.Fields.IsModified("DlpPolicy"))
			{
				return true;
			}
			Guid immutableId;
			if (!Guid.TryParse(this.DlpPolicy, out immutableId))
			{
				ADComplianceProgram adcomplianceProgram = DlpUtils.GetInstalledTenantDlpPolicies(base.DataSession, this.DlpPolicy).FirstOrDefault<ADComplianceProgram>();
				if (adcomplianceProgram == null)
				{
					return false;
				}
				immutableId = adcomplianceProgram.ImmutableId;
			}
			Guid guid;
			if (rule.TryGetDlpPolicyId(out guid))
			{
				return guid.Equals(immutableId);
			}
			return string.IsNullOrEmpty(this.DlpPolicy);
		}

		private bool FilterMatches(Rule rule)
		{
			if (!base.Fields.IsModified("Filter") || base.Fields["Filter"] == null)
			{
				return true;
			}
			if (this.searchFilter == null)
			{
				ExAssert.RetailAssert(false, "Internal error - search filter is not set");
				return false;
			}
			RuleDescription ruleDescription = Utils.BuildRuleDescription(rule, int.MaxValue);
			bool result;
			using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(ruleDescription.ToString())))
			{
				result = this.searchFilter.IsMatch(new TextScanContext(stream));
			}
			return result;
		}

		internal static IMatch ParseFilterParameter(string filterParameter, out string error)
		{
			error = null;
			if (!Regex.IsMatch(filterParameter, "Description\\s+-like\\s+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
			{
				error = Strings.InvalidRuleSearchFilterMissingElements;
				return null;
			}
			Regex regex = new Regex(string.Format("Description\\s+-like\\s+(\"|')(?<{0}>.*)(\"|')", "theFilter"), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			Match match = regex.Match(filterParameter);
			if (match.Groups["theFilter"] != null)
			{
				string text = regex.Match(filterParameter).Groups["theFilter"].Value.Trim();
				if (string.IsNullOrEmpty(text))
				{
					error = Strings.InvalidRuleSearchFilterEmpty;
					return null;
				}
				MatchFactory matchFactory = new MatchFactory();
				try
				{
					return matchFactory.CreateRegex(GetTransportRule.FilterStringToRegex(text), false, false);
				}
				catch (ArgumentException ex)
				{
					error = (string.IsNullOrWhiteSpace(ex.Message) ? "Unknown Error" : ex.Message);
					return null;
				}
			}
			error = Strings.InvalidRuleSearchFilterEmpty;
			return null;
		}

		internal static string FilterStringToRegex(string filterString)
		{
			int length = filterString.Length;
			string text = filterString.TrimStart(new char[]
			{
				'*'
			});
			if (length == text.Length)
			{
				text = "D46AD88A-73C2-4953-BBF8-11D5FF2BF2F2" + text;
			}
			length = text.Length;
			text = text.TrimEnd(new char[]
			{
				'*'
			});
			if (length == text.Length)
			{
				text += "D46AD88A-73C2-4953-BBF8-11D5FF2BF2F2";
			}
			text = text.Replace("\\*", "7494EA36-CE2A-46A2-91C2-6B2D432F1543");
			text = text.Replace("*", "03F57514-108F-49BE-8683-F0E2A9BB64D");
			text = Regex.Escape(text);
			text = text.Replace("D46AD88A-73C2-4953-BBF8-11D5FF2BF2F2", "\\b");
			text = text.Replace("03F57514-108F-49BE-8683-F0E2A9BB64D", ".*");
			return text.Replace("7494EA36-CE2A-46A2-91C2-6B2D432F1543", "\\*");
		}

		private readonly string ruleCollectionName;

		private readonly TypeMapping[] supportedPredicates;

		private readonly TypeMapping[] supportedActions;

		private IMatch searchFilter;
	}
}
