using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class SetSubject : TransportAction
	{
		public SetSubject(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "SetSubject";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return SetSubject.argumentTypes;
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
			string subject = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			transportRulesEvaluationContext.MailItem.Message.Subject = subject;
			return ExecutionControl.Execute;
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string)
		};
	}
}
