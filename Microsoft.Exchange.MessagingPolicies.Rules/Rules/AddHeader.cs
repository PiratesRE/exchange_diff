using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AddHeader : HeaderValueAction
	{
		public AddHeader(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "AddHeader";
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			string headerName = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			string value = (string)base.Arguments[1].GetValue(transportRulesEvaluationContext);
			TransportUtils.AddHeaderToMail(transportRulesEvaluationContext.MailItem.Message, headerName, value);
			return ExecutionControl.Execute;
		}
	}
}
