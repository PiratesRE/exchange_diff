using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal abstract class ClientAccessRuleAction : Microsoft.Exchange.MessagingPolicies.Rules.Action
	{
		public ClientAccessRuleAction(ShortList<Argument> arguments) : base(arguments)
		{
		}
	}
}
