using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class SetExtendedPropertyString : TransportAction
	{
		public SetExtendedPropertyString(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return SetExtendedPropertyString.argumentTypes;
			}
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.BifurcationNeeded;
			}
		}

		public override string Name
		{
			get
			{
				return "SetExtendedPropertyString";
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			string key = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			string value = (string)base.Arguments[1].GetValue(transportRulesEvaluationContext);
			MailItem mailItem = transportRulesEvaluationContext.MailItem;
			mailItem.Properties[key] = value;
			return ExecutionControl.Execute;
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string),
			typeof(string)
		};
	}
}
