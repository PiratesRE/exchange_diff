using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class LogEvent : TransportAction
	{
		public LogEvent(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return LogEvent.argumentTypes;
			}
		}

		public override string Name
		{
			get
			{
				return "LogEvent";
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext context = (TransportRulesEvaluationContext)baseContext;
			string text = (string)base.Arguments[0].GetValue(context);
			TransportAction.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_RuleActionLogEvent, null, new object[]
			{
				text
			});
			return ExecutionControl.Execute;
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string)
		};
	}
}
