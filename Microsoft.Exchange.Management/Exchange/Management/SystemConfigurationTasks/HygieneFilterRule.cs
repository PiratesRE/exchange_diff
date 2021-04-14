using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class HygieneFilterRule : RulePresentationObjectBase
	{
		protected HygieneFilterRule()
		{
		}

		protected HygieneFilterRule(TransportRule transportRule, string name, int priority, RuleState state, string comments, TransportRulePredicate[] conditions, TransportRulePredicate[] exceptions, ADIdParameter policyId) : base(transportRule)
		{
			if (transportRule == null)
			{
				base.Name = name;
			}
			this.Priority = priority;
			this.State = state;
			this.Comments = comments;
			this.conditions = conditions;
			this.exceptions = exceptions;
			this.PolicyId = policyId;
		}

		public RuleState State { get; set; }

		public int Priority { get; set; }

		public string Comments { get; set; }

		public RuleDescription Description
		{
			get
			{
				return HygieneFilterRule.BuildRuleDescription(this);
			}
		}

		public Version RuleVersion
		{
			get
			{
				if (string.IsNullOrEmpty(base.TransportRuleXml))
				{
					return null;
				}
				Version result;
				try
				{
					TransportRule transportRule = (TransportRule)TransportRuleParser.Instance.GetRule(base.TransportRuleXml);
					result = (transportRule.IsTooAdvancedToParse ? null : transportRule.MinimumVersion);
				}
				catch (ParserException)
				{
					result = null;
				}
				return result;
			}
		}

		public RecipientIdParameter[] SentTo
		{
			get
			{
				return this.sentTo;
			}
			set
			{
				this.sentTo = value;
			}
		}

		public RecipientIdParameter[] SentToMemberOf
		{
			get
			{
				return this.sentToMemberOf;
			}
			set
			{
				this.sentToMemberOf = value;
			}
		}

		public Word[] RecipientDomainIs
		{
			get
			{
				return this.recipientDomainIs;
			}
			set
			{
				this.recipientDomainIs = value;
			}
		}

		public RecipientIdParameter[] ExceptIfSentTo
		{
			get
			{
				return this.exceptIfSentTo;
			}
			set
			{
				this.exceptIfSentTo = value;
			}
		}

		public RecipientIdParameter[] ExceptIfSentToMemberOf
		{
			get
			{
				return this.exceptIfSentToMemberOf;
			}
			set
			{
				this.exceptIfSentToMemberOf = value;
			}
		}

		public Word[] ExceptIfRecipientDomainIs
		{
			get
			{
				return this.exceptIfRecipientDomainIs;
			}
			set
			{
				this.exceptIfRecipientDomainIs = value;
			}
		}

		public TransportRulePredicate[] Conditions
		{
			get
			{
				return this.conditions;
			}
			set
			{
				this.conditions = value;
			}
		}

		public TransportRulePredicate[] Exceptions
		{
			get
			{
				return this.exceptions;
			}
			set
			{
				this.exceptions = value;
			}
		}

		public override ValidationError[] Validate()
		{
			if (!this.isValid)
			{
				return new ValidationError[]
				{
					new ObjectValidationError(this.errorText, base.Identity, null)
				};
			}
			return ValidationError.None;
		}

		protected ADIdParameter PolicyId { get; set; }

		protected void SetParametersFromPredicate(TransportRulePredicate predicate, bool isException)
		{
			if (predicate is SentToPredicate)
			{
				if (isException)
				{
					this.exceptIfSentTo = Utils.BuildRecipientIdArray(((SentToPredicate)predicate).Addresses);
					return;
				}
				this.sentTo = Utils.BuildRecipientIdArray(((SentToPredicate)predicate).Addresses);
				return;
			}
			else
			{
				if (!(predicate is SentToMemberOfPredicate))
				{
					if (predicate is RecipientDomainIsPredicate)
					{
						if (isException)
						{
							this.exceptIfRecipientDomainIs = ((RecipientDomainIsPredicate)predicate).Words;
							return;
						}
						this.recipientDomainIs = ((RecipientDomainIsPredicate)predicate).Words;
					}
					return;
				}
				if (isException)
				{
					this.exceptIfSentToMemberOf = Utils.BuildRecipientIdArray(((SentToMemberOfPredicate)predicate).Addresses);
					return;
				}
				this.sentToMemberOf = Utils.BuildRecipientIdArray(((SentToMemberOfPredicate)predicate).Addresses);
				return;
			}
		}

		internal abstract IEnumerable<Microsoft.Exchange.MessagingPolicies.Rules.Action> CreateActions();

		internal abstract string BuildActionDescription();

		internal void SetPolicyId(ADIdParameter policyId)
		{
			this.PolicyId = policyId;
		}

		internal TransportRule ToInternalRule()
		{
			AndCondition andCondition = new AndCondition();
			List<RuleBifurcationInfo> list = new List<RuleBifurcationInfo>();
			andCondition.SubConditions.Add(Microsoft.Exchange.MessagingPolicies.Rules.Condition.True);
			int num = -1;
			if (this.conditions != null)
			{
				foreach (TransportRulePredicate transportRulePredicate in this.conditions)
				{
					if (transportRulePredicate.Rank <= num)
					{
						throw new ArgumentException(Strings.InvalidPredicateSequence, "Conditions");
					}
					num = transportRulePredicate.Rank;
					if (transportRulePredicate is BifurcationInfoPredicate)
					{
						BifurcationInfoPredicate bifurcationInfoPredicate = (BifurcationInfoPredicate)transportRulePredicate;
						RuleBifurcationInfo ruleBifurcationInfo;
						RuleBifurcationInfo item = bifurcationInfoPredicate.ToRuleBifurcationInfo(out ruleBifurcationInfo);
						list.Add(item);
						if (ruleBifurcationInfo != null)
						{
							list.Add(ruleBifurcationInfo);
						}
					}
					else
					{
						Microsoft.Exchange.MessagingPolicies.Rules.Condition item2 = transportRulePredicate.ToInternalCondition();
						andCondition.SubConditions.Add(item2);
					}
				}
			}
			if (this.exceptions != null && this.exceptions.Length > 0)
			{
				OrCondition orCondition = new OrCondition();
				andCondition.SubConditions.Add(new NotCondition(orCondition));
				num = -1;
				foreach (TransportRulePredicate transportRulePredicate2 in this.exceptions)
				{
					if (transportRulePredicate2.Rank <= num)
					{
						throw new ArgumentException(Strings.InvalidPredicateSequence, "Exceptions");
					}
					num = transportRulePredicate2.Rank;
					if (transportRulePredicate2 is BifurcationInfoPredicate)
					{
						BifurcationInfoPredicate bifurcationInfoPredicate2 = (BifurcationInfoPredicate)transportRulePredicate2;
						RuleBifurcationInfo ruleBifurcationInfo3;
						RuleBifurcationInfo ruleBifurcationInfo2 = bifurcationInfoPredicate2.ToRuleBifurcationInfo(out ruleBifurcationInfo3);
						ruleBifurcationInfo2.Exception = true;
						list.Add(ruleBifurcationInfo2);
						if (ruleBifurcationInfo3 != null)
						{
							ruleBifurcationInfo3.Exception = true;
							list.Add(ruleBifurcationInfo3);
						}
					}
					else
					{
						Microsoft.Exchange.MessagingPolicies.Rules.Condition item3 = transportRulePredicate2.ToInternalCondition();
						orCondition.SubConditions.Add(item3);
					}
				}
				if (orCondition.SubConditions.Count == 0)
				{
					orCondition.SubConditions.Add(Microsoft.Exchange.MessagingPolicies.Rules.Condition.False);
				}
			}
			TransportRule transportRule = new TransportRule(base.Name, andCondition);
			transportRule.Enabled = this.State;
			transportRule.Comments = this.Comments;
			if (list.Count > 0)
			{
				transportRule.Fork = list;
			}
			foreach (Microsoft.Exchange.MessagingPolicies.Rules.Action item4 in this.CreateActions())
			{
				transportRule.Actions.Add(item4);
			}
			return transportRule;
		}

		internal override void SuppressPiiData(PiiMap piiMap)
		{
			base.SuppressPiiData(piiMap);
			if (this.conditions != null)
			{
				foreach (TransportRulePredicate transportRulePredicate in this.conditions)
				{
					transportRulePredicate.SuppressPiiData();
				}
			}
			if (this.exceptions != null)
			{
				foreach (TransportRulePredicate transportRulePredicate2 in this.exceptions)
				{
					transportRulePredicate2.SuppressPiiData();
				}
			}
			this.sentTo = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(HygieneFilterRuleSchema.SentTo, this.sentTo);
			this.sentToMemberOf = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(HygieneFilterRuleSchema.SentToMemberOf, this.sentToMemberOf);
			this.recipientDomainIs = SuppressingPiiProperty.TryRedactValue<Word[]>(HygieneFilterRuleSchema.RecipientDomainIs, this.recipientDomainIs);
			this.exceptIfSentTo = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(HygieneFilterRuleSchema.ExceptIfSentTo, this.exceptIfSentTo);
			this.exceptIfSentToMemberOf = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(HygieneFilterRuleSchema.ExceptIfSentToMemberOf, this.exceptIfSentToMemberOf);
			this.exceptIfRecipientDomainIs = SuppressingPiiProperty.TryRedactValue<Word[]>(HygieneFilterRuleSchema.ExceptIfRecipientDomainIs, this.exceptIfRecipientDomainIs);
		}

		private static RuleDescription BuildRuleDescription(HygieneFilterRule rule)
		{
			RuleDescription ruleDescription = new RuleDescription();
			if (rule.Conditions != null && rule.Conditions.Length > 0)
			{
				foreach (TransportRulePredicate transportRulePredicate in rule.Conditions)
				{
					ruleDescription.ConditionDescriptions.Add(transportRulePredicate.Description);
				}
			}
			ruleDescription.ActionDescriptions.Add(rule.BuildActionDescription());
			if (rule.Exceptions != null && rule.Exceptions.Length > 0)
			{
				foreach (TransportRulePredicate transportRulePredicate2 in rule.Exceptions)
				{
					ruleDescription.ExceptionDescriptions.Add(transportRulePredicate2.Description);
				}
			}
			return ruleDescription;
		}

		internal const string MalwareFilterVersioned = "MalwareFilterVersioned";

		internal const string HostedContentFilterVersioned = "HostedContentFilterVersioned";

		internal const string XMSExchangeMalwareFilterPolicyHeader = "X-MS-Exchange-Organization-MalwareFilterPolicy";

		internal const string XMSExchangeHostedContentFilterPolicyHeader = "X-MS-Exchange-Organization-HostedContentFilterPolicy";

		private RecipientIdParameter[] sentTo;

		private RecipientIdParameter[] sentToMemberOf;

		private Word[] recipientDomainIs;

		private RecipientIdParameter[] exceptIfSentTo;

		private RecipientIdParameter[] exceptIfSentToMemberOf;

		private Word[] exceptIfRecipientDomainIs;

		private TransportRulePredicate[] conditions;

		private TransportRulePredicate[] exceptions;

		protected LocalizedString errorText = LocalizedString.Empty;

		internal static TypeMapping[] SupportedPredicates = new TypeMapping[]
		{
			new TypeMapping("SentTo", typeof(SentToPredicate), RulesTasksStrings.LinkedPredicateSentTo, RulesTasksStrings.LinkedPredicateSentToException),
			new TypeMapping("SentToMemberOf", typeof(SentToMemberOfPredicate), RulesTasksStrings.LinkedPredicateSentToMemberOf, RulesTasksStrings.LinkedPredicateSentToMemberOfException),
			new TypeMapping("RecipientDomainIs", typeof(RecipientDomainIsPredicate), RulesTasksStrings.LinkedPredicateRecipientDomainIs, RulesTasksStrings.LinkedPredicateRecipientDomainIsException)
		};

		internal static TypeMapping[] SupportedActions = new TypeMapping[]
		{
			new TypeMapping("SetHeader", typeof(SetHeaderAction), RulesTasksStrings.LinkedActionSetHeader),
			new TypeMapping("StopRuleProcessing", typeof(StopRuleProcessingAction), RulesTasksStrings.LinkedActionStopRuleProcessing)
		};
	}
}
