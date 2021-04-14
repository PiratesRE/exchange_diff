using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class EnableHygieneFilterRuleTaskBase : EnableRuleTaskBase
	{
		protected EnableHygieneFilterRuleTaskBase(string ruleCollectionName) : base(ruleCollectionName)
		{
		}

		protected override IConfigurable PrepareDataObject()
		{
			TransportRule transportRule = (TransportRule)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			TransportRule transportRule2 = (TransportRule)TransportRuleParser.Instance.GetRule(transportRule.Xml);
			if (transportRule2.IsTooAdvancedToParse)
			{
				base.WriteError(new InvalidOperationException(Strings.CannotModifyRuleDueToVersion(transportRule2.Name)), ErrorCategory.InvalidOperation, null);
			}
			transportRule2.Enabled = RuleState.Enabled;
			transportRule.Xml = TransportRuleSerializer.Instance.SaveRuleToString(transportRule2);
			return transportRule;
		}
	}
}
