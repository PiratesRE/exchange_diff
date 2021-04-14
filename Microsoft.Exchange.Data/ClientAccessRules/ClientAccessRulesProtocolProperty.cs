using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class ClientAccessRulesProtocolProperty : Property
	{
		public ClientAccessRulesProtocolProperty(string propertyName, Type type) : base(propertyName, type)
		{
		}

		protected override object OnGetValue(RulesEvaluationContext baseContext)
		{
			ClientAccessRulesEvaluationContext clientAccessRulesEvaluationContext = (ClientAccessRulesEvaluationContext)baseContext;
			return clientAccessRulesEvaluationContext.Protocol;
		}

		public const string PropertyName = "ProtocolProperty";
	}
}
