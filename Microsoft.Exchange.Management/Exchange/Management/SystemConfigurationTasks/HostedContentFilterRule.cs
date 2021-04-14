using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class HostedContentFilterRule : HygieneFilterRule
	{
		public HostedContentFilterRule()
		{
		}

		internal HostedContentFilterRule(TransportRule transportRule, string name, int priority, RuleState state, string comments, TransportRulePredicate[] conditions, TransportRulePredicate[] exceptions, HostedContentFilterPolicyIdParameter policyId) : base(transportRule, name, priority, state, comments, conditions, exceptions, policyId)
		{
			if (base.Conditions != null)
			{
				foreach (TransportRulePredicate predicate in base.Conditions)
				{
					base.SetParametersFromPredicate(predicate, false);
				}
			}
			if (base.Exceptions != null)
			{
				foreach (TransportRulePredicate predicate2 in base.Exceptions)
				{
					base.SetParametersFromPredicate(predicate2, true);
				}
			}
		}

		internal ObjectSchema Schema
		{
			get
			{
				return HostedContentFilterRule.schema;
			}
		}

		internal bool IsEsnCompatible
		{
			get
			{
				if (base.Exceptions != null && base.Exceptions.Length > 0)
				{
					return false;
				}
				return base.Conditions.All((TransportRulePredicate condition) => condition is RecipientDomainIsPredicate);
			}
		}

		public HostedContentFilterPolicyIdParameter HostedContentFilterPolicy
		{
			get
			{
				return base.PolicyId as HostedContentFilterPolicyIdParameter;
			}
			set
			{
				base.PolicyId = value;
			}
		}

		internal override IEnumerable<Microsoft.Exchange.MessagingPolicies.Rules.Action> CreateActions()
		{
			List<Microsoft.Exchange.MessagingPolicies.Rules.Action> list = new List<Microsoft.Exchange.MessagingPolicies.Rules.Action>();
			ShortList<Argument> arguments = new ShortList<Argument>
			{
				new Value("X-MS-Exchange-Organization-HostedContentFilterPolicy"),
				new Value(base.PolicyId.ToString())
			};
			list.Add(TransportRuleParser.Instance.CreateAction("SetHeader", arguments, null));
			list.Add(TransportRuleParser.Instance.CreateAction("Halt", new ShortList<Argument>(), null));
			return list;
		}

		internal override string BuildActionDescription()
		{
			return Strings.HostedContentFilterActionDescription((base.PolicyId == null) ? string.Empty : base.PolicyId.ToString());
		}

		internal override void SuppressPiiData(PiiMap piiMap)
		{
			base.SuppressPiiData(piiMap);
			if (base.PolicyId != null)
			{
				base.PolicyId = SuppressingPiiProperty.TryRedactValue<ADIdParameter>(HostedContentFilterRuleSchema.HostedContentFilterPolicy, base.PolicyId);
			}
		}

		internal static HostedContentFilterRule CreateFromInternalRule(TransportRule rule, int priority, TransportRule transportRule)
		{
			IConfigDataProvider session = null;
			if (transportRule != null)
			{
				session = transportRule.Session;
			}
			TransportRulePredicate[] conditions;
			TransportRulePredicate[] exceptions;
			TransportRuleAction[] array;
			Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Rule.TryConvert(HygieneFilterRule.SupportedPredicates, HygieneFilterRule.SupportedActions, rule, out conditions, out exceptions, out array, session);
			if (array == null || array.Length != 2 || !(array[0] is SetHeaderAction) || !(array[1] is StopRuleProcessingAction))
			{
				throw new CorruptFilterRuleException(Strings.CorruptRule(rule.Name, Strings.ErrorCorruptRuleAction));
			}
			string identity = ((SetHeaderAction)array[0]).HeaderValue.ToString();
			return new HostedContentFilterRule(transportRule, rule.Name, priority, rule.Enabled, rule.Comments, conditions, exceptions, new HostedContentFilterPolicyIdParameter(identity));
		}

		internal static HostedContentFilterRule CreateCorruptRule(int priority, TransportRule transportRule, LocalizedString errorText)
		{
			return new HostedContentFilterRule(transportRule, transportRule.Name, priority, RuleState.Disabled, null, null, null, null)
			{
				isValid = false,
				errorText = errorText
			};
		}

		private static readonly HostedContentFilterRuleSchema schema = ObjectSchema.GetInstance<HostedContentFilterRuleSchema>();
	}
}
