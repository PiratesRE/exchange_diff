using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "HostedContentFilterRule", DefaultParameterSetName = "Identity")]
	public sealed class GetHostedContentFilterRule : GetHygieneFilterRuleTaskBase
	{
		public GetHostedContentFilterRule() : base("HostedContentFilterVersioned")
		{
		}

		internal override IConfigurable CreateCorruptTaskRule(int priority, TransportRule transportRule, string errorMessage)
		{
			return HostedContentFilterRule.CreateCorruptRule(priority, transportRule, Strings.CorruptRule(transportRule.Name, errorMessage));
		}

		internal override IConfigurable CreateTaskRuleFromInternalRule(TransportRule rule, int priority, TransportRule transportRule)
		{
			HostedContentFilterRule result;
			try
			{
				result = HostedContentFilterRule.CreateFromInternalRule(rule, priority, transportRule);
			}
			catch (CorruptFilterRuleException ex)
			{
				result = HostedContentFilterRule.CreateCorruptRule(priority, transportRule, ex.LocalizedString);
			}
			return result;
		}
	}
}
