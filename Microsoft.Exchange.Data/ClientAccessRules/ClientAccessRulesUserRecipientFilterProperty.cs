using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class ClientAccessRulesUserRecipientFilterProperty : Property
	{
		public ClientAccessRulesUserRecipientFilterProperty(string propertyName, Type type) : base(propertyName, type)
		{
		}

		protected override object OnGetValue(RulesEvaluationContext baseContext)
		{
			ClientAccessRulesEvaluationContext clientAccessRulesEvaluationContext = (ClientAccessRulesEvaluationContext)baseContext;
			return clientAccessRulesEvaluationContext.User;
		}

		public const string PropertyName = "UserRecipientFilterProperty";
	}
}
