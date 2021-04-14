using System;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Serializable]
	public class UMCallAnsweringRuleDescription : RuleDescription
	{
		internal override string RuleDescriptionIf
		{
			get
			{
				return Strings.UMCallAnsweringRuleDescriptionIf;
			}
		}

		internal override string RuleDescriptionTakeActions
		{
			get
			{
				return Strings.UMCallAnsweringRuleDescriptionTakeActions;
			}
		}
	}
}
