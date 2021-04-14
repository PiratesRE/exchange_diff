using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class ClientAccessRulesAuthenticationTypeProperty : Property
	{
		public ClientAccessRulesAuthenticationTypeProperty(string propertyName, Type type) : base(propertyName, type)
		{
		}

		protected override object OnGetValue(RulesEvaluationContext baseContext)
		{
			ClientAccessRulesEvaluationContext clientAccessRulesEvaluationContext = (ClientAccessRulesEvaluationContext)baseContext;
			return clientAccessRulesEvaluationContext.AuthenticationType;
		}

		public const string PropertyName = "AuthenticationTypeProperty";
	}
}
