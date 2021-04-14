using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class ExtendedProperty : Property
	{
		public ExtendedProperty(string propertyName, Type type) : base(propertyName, type)
		{
		}

		protected override object OnGetValue(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			object obj = null;
			if (transportRulesEvaluationContext.Message.ExtendedProperties.TryGetValue(base.Name, out obj))
			{
				TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, string.Format("{0} extended property value evaluated as rule condition: '{1}'", base.Name, obj));
				return obj;
			}
			TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, string.Format("{0} extended property value evaluated 'null'", base.Name));
			return null;
		}

		public const string Prefix = "Message.ExtendedProperties";
	}
}
