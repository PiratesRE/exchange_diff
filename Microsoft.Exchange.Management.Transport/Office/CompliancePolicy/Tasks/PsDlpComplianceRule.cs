using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public sealed class PsDlpComplianceRule : PsComplianceRuleBase
	{
		public PsDlpComplianceRule()
		{
		}

		public PsDlpComplianceRule(RuleStorage ruleStorage) : base(ruleStorage)
		{
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return PsDlpComplianceRule.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		public Hashtable[] ContentContainsSensitiveInformation { get; internal set; }

		public AccessScope? AccessScopeIs
		{
			get
			{
				return (AccessScope?)this[PsDlpComplianceRuleSchema.AccessScopeIs];
			}
			set
			{
				this[PsDlpComplianceRuleSchema.AccessScopeIs] = value;
			}
		}

		public MultiValuedProperty<string> ContentPropertyContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[PsDlpComplianceRuleSchema.ContentPropertyContainsWords];
			}
			set
			{
				this[PsDlpComplianceRuleSchema.ContentPropertyContainsWords] = value;
			}
		}

		public bool BlockAccess
		{
			get
			{
				return (bool)this[PsDlpComplianceRuleSchema.BlockAccess];
			}
			set
			{
				this[PsDlpComplianceRuleSchema.BlockAccess] = value;
			}
		}

		internal override void PopulateTaskProperties(Task task, IConfigurationSession configurationSession)
		{
			base.PopulateTaskProperties(task, configurationSession);
			if (!string.IsNullOrEmpty(base.RuleBlob))
			{
				PolicyRule policyRuleFromRuleBlob = this.GetPolicyRuleFromRuleBlob();
				if (policyRuleFromRuleBlob.IsTooAdvancedToParse)
				{
					base.ReadOnly = true;
				}
				else
				{
					this.SetTaskConditions(PsDlpComplianceRule.ConvertEngineConditionToTaskConditions(policyRuleFromRuleBlob.Condition));
					this.SetTaskActions(PsDlpComplianceRule.ConvertEngineActionsToTaskActions(policyRuleFromRuleBlob.Actions));
				}
				base.ResetChangeTracking();
			}
		}

		internal override void UpdateStorageProperties(Task task, IConfigurationSession configurationSession, bool isNewRule)
		{
			base.UpdateStorageProperties(task, configurationSession, isNewRule);
			if (base.ObjectState != ObjectState.Unchanged)
			{
				PolicyRule policyRule = new PolicyRule
				{
					Condition = PsDlpComplianceRule.ConvertTaskConditionsToEngineCondition(this.GetTaskConditions()),
					Actions = PsDlpComplianceRule.ConvertTaskActionsToEngineActions(this.GetTaskActions()),
					Comments = base.Comment,
					Enabled = (base.Enabled ? RuleState.Enabled : RuleState.Disabled),
					ImmutableId = base.Guid,
					Name = base.Name
				};
				base.RuleBlob = this.GetRuleXmlFromPolicyRule(policyRule);
			}
		}

		internal override PolicyRule GetPolicyRuleFromRuleBlob()
		{
			RuleParser ruleParser = new RuleParser(new SimplePolicyParserFactory());
			return ruleParser.GetRule(base.RuleBlob);
		}

		private IEnumerable<PsComplianceRulePredicateBase> GetTaskConditions()
		{
			List<PsComplianceRulePredicateBase> list = new List<PsComplianceRulePredicateBase>();
			if (this.AccessScopeIs != null)
			{
				list.Add(new PsAccessScopeIsPredicate(this.AccessScopeIs.Value));
			}
			if (this.ContentContainsSensitiveInformation != null && this.ContentContainsSensitiveInformation.Any<Hashtable>())
			{
				list.Add(new PsContentContainsSensitiveInformationPredicate(this.ContentContainsSensitiveInformation));
			}
			if (this.ContentPropertyContainsWords != null && this.ContentPropertyContainsWords.Any<string>())
			{
				list.Add(new PsContentPropertyContainsWordsPredicate(this.ContentPropertyContainsWords));
			}
			return list;
		}

		private void SetTaskConditions(IEnumerable<PsComplianceRulePredicateBase> conditions)
		{
			foreach (PsComplianceRulePredicateBase psComplianceRulePredicateBase in conditions)
			{
				if (psComplianceRulePredicateBase is PsAccessScopeIsPredicate)
				{
					this.AccessScopeIs = new AccessScope?((psComplianceRulePredicateBase as PsAccessScopeIsPredicate).PropertyValue);
				}
				else if (psComplianceRulePredicateBase is PsContentContainsSensitiveInformationPredicate)
				{
					PsContentContainsSensitiveInformationPredicate psContentContainsSensitiveInformationPredicate = psComplianceRulePredicateBase as PsContentContainsSensitiveInformationPredicate;
					this.ContentContainsSensitiveInformation = psContentContainsSensitiveInformationPredicate.DataClassifications;
				}
				else
				{
					if (!(psComplianceRulePredicateBase is PsContentPropertyContainsWordsPredicate))
					{
						throw new UnexpectedConditionOrActionDetectedException();
					}
					PsContentPropertyContainsWordsPredicate psContentPropertyContainsWordsPredicate = psComplianceRulePredicateBase as PsContentPropertyContainsWordsPredicate;
					this.ContentPropertyContainsWords = psContentPropertyContainsWordsPredicate.Words;
				}
			}
		}

		internal IEnumerable<PsComplianceRuleActionBase> GetTaskActions()
		{
			List<PsComplianceRuleActionBase> list = new List<PsComplianceRuleActionBase>();
			if (this.BlockAccess)
			{
				list.Add(new PsBlockAccessAction());
			}
			return list;
		}

		private void SetTaskActions(IEnumerable<PsComplianceRuleActionBase> actions)
		{
			foreach (PsComplianceRuleActionBase psComplianceRuleActionBase in actions)
			{
				if (!(psComplianceRuleActionBase is PsBlockAccessAction))
				{
					throw new UnexpectedConditionOrActionDetectedException();
				}
				this.BlockAccess = true;
			}
		}

		internal static IEnumerable<PsComplianceRulePredicateBase> ConvertEngineConditionToTaskConditions(Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition condition)
		{
			List<PsComplianceRulePredicateBase> list = new List<PsComplianceRulePredicateBase>();
			if (condition is AndCondition)
			{
				AndCondition andCondition = condition as AndCondition;
				using (List<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition>.Enumerator enumerator = andCondition.SubConditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition condition2 = enumerator.Current;
						if (condition2 is PredicateCondition)
						{
							list.Add(PsComplianceRulePredicateBase.FromEnginePredicate(condition2 as PredicateCondition));
						}
						else if (!(condition2 is TrueCondition))
						{
							throw new UnexpectedConditionOrActionDetectedException();
						}
					}
					return list;
				}
			}
			throw new UnexpectedConditionOrActionDetectedException();
		}

		internal static Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition ConvertTaskConditionsToEngineCondition(IEnumerable<PsComplianceRulePredicateBase> predicates)
		{
			AndCondition andCondition = new AndCondition();
			foreach (PsComplianceRulePredicateBase psComplianceRulePredicateBase in predicates)
			{
				andCondition.SubConditions.Add(psComplianceRulePredicateBase.ToEnginePredicate());
			}
			if (!andCondition.SubConditions.Any<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition>())
			{
				andCondition.SubConditions.Add(new TrueCondition());
			}
			return andCondition;
		}

		internal static IEnumerable<PsComplianceRuleActionBase> ConvertEngineActionsToTaskActions(IList<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action> actions)
		{
			return actions.Select(new Func<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action, PsComplianceRuleActionBase>(PsComplianceRuleActionBase.FromEngineAction));
		}

		internal static IList<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action> ConvertTaskActionsToEngineActions(IEnumerable<PsComplianceRuleActionBase> actions)
		{
			return (from action in actions
			select action.ToEngineAction()).ToList<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action>();
		}

		private static readonly PsDlpComplianceRuleSchema schema = ObjectSchema.GetInstance<PsDlpComplianceRuleSchema>();
	}
}
