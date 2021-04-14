using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MessagingPolicies;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class GetHygieneFilterRuleTaskBase : GetMultitenancySystemConfigurationObjectTask<RuleIdParameter, TransportRule>
	{
		protected GetHygieneFilterRuleTaskBase(string ruleCollectionName)
		{
			this.ruleCollectionName = ruleCollectionName;
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

		protected override ObjectId RootId
		{
			get
			{
				if (this.Identity == null)
				{
					return RuleIdParameter.GetRuleCollectionId(base.DataSession, this.ruleCollectionName);
				}
				return null;
			}
		}

		protected override void InternalValidate()
		{
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = RuleIdParameter.GetRuleCollectionRdn(this.ruleCollectionName);
			}
			base.InternalValidate();
		}

		internal abstract IConfigurable CreateCorruptTaskRule(int priority, TransportRule transportRule, string errorText);

		internal abstract IConfigurable CreateTaskRuleFromInternalRule(TransportRule rule, int priority, TransportRule transportRule);

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
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 169, "GetRuleCollections", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageHygiene\\HygieneConfiguration\\GetHygieneFilterRuleTaskBase.cs");
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
			string errorText = string.Empty;
			if (base.NeedSuppressingPiiData)
			{
				base.ExchangeRunspaceConfig.EnablePiiMap = true;
			}
			try
			{
				transportRule2 = (TransportRule)TransportRuleParser.Instance.GetRule(transportRule.Xml);
			}
			catch (ParserException ex)
			{
				errorText = ex.Message;
			}
			if (transportRule2 == null)
			{
				this.WriteResult(this.CreateCorruptTaskRule(priority, transportRule, errorText));
				return;
			}
			if (this.StateMatches(transportRule2))
			{
				IConfigurable configurable = this.CreateTaskRuleFromInternalRule(transportRule2, priority, transportRule);
				if (base.NeedSuppressingPiiData)
				{
					((HygieneFilterRule)configurable).SuppressPiiData(Utils.GetSessionPiiMap(base.ExchangeRunspaceConfig));
				}
				this.WriteResult(configurable);
			}
		}

		private bool StateMatches(TransportRule rule)
		{
			return !base.Fields.IsModified("State") || this.State == rule.Enabled;
		}

		private readonly string ruleCollectionName;
	}
}
