using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class BodyProperty : MessageProperty
	{
		public BodyProperty() : base("Message.Body", typeof(IContent))
		{
		}

		protected override object OnGetValue(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, "Body property value is evaluated as rule condition");
			return transportRulesEvaluationContext.Message.Body;
		}

		public const string PropertyName = "Message.Body";
	}
}
