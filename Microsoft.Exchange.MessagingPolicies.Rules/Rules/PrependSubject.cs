using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class PrependSubject : TransportAction
	{
		public PrependSubject(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "PrependSubject";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return PrependSubject.argumentTypes;
			}
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.BifurcationNeeded;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			string subject = transportRulesEvaluationContext.MailItem.Message.Subject;
			string text = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			if (string.IsNullOrEmpty(subject))
			{
				transportRulesEvaluationContext.MailItem.Message.Subject = text;
			}
			else if (!subject.StartsWith(text, StringComparison.OrdinalIgnoreCase))
			{
				transportRulesEvaluationContext.MailItem.Message.Subject = text + subject;
			}
			baseContext.ResetTextProcessingContext("Message.Subject", false);
			return ExecutionControl.Execute;
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string)
		};
	}
}
