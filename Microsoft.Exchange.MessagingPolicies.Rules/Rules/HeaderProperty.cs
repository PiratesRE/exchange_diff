using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class HeaderProperty : Property
	{
		public HeaderProperty(string headerName) : base(headerName, typeof(List<string>))
		{
			if (!TransportUtils.IsHeaderValid(headerName))
			{
				throw new RulesValidationException(TransportRulesStrings.InvalidHeaderName(headerName));
			}
		}

		protected override object OnGetValue(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			object obj = transportRulesEvaluationContext.Message.Headers[base.Name];
			TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, string.Format("Header property value evaluated as rule condition: '{0}'", obj ?? "null"));
			if (transportRulesEvaluationContext.IsTestMessage && obj != null)
			{
				TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, string.Format("property is a collection of values: '{0}'", string.Join(",", obj as IEnumerable<string>)));
			}
			return obj;
		}

		public const string Prefix = "Message.Headers";

		public const string TypeName = "List<string>";
	}
}
