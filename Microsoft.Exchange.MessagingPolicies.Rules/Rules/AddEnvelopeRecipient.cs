using System;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AddEnvelopeRecipient : TransportAction
	{
		public AddEnvelopeRecipient(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "AddEnvelopeRecipient";
			}
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.NonRecipientRelated;
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return AddEnvelopeRecipient.argumentTypes;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			string text = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<string>(0L, "AddEnvelopeRecipient: {0}", text);
			transportRulesEvaluationContext.RecipientsToAdd.Add(new TransportRulesEvaluationContext.AddedRecipient(text, text, RecipientP2Type.Bcc));
			return ExecutionControl.Execute;
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string)
		};
	}
}
