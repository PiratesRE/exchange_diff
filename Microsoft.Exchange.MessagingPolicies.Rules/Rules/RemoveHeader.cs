using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class RemoveHeader : TransportAction
	{
		public RemoveHeader(ShortList<Argument> arguments) : base(arguments)
		{
			if (!(base.Arguments[0] is Value))
			{
				throw new RulesValidationException(RulesStrings.ActionArgumentMismatch(this.Name));
			}
			string text = (string)base.Arguments[0].GetValue(null);
			if ("X-MS-Exchange-Inbox-Rules-Loop".Equals(text, StringComparison.OrdinalIgnoreCase) || "X-MS-Exchange-Transport-Rules-Loop".Equals(text, StringComparison.OrdinalIgnoreCase) || "X-MS-Gcc-Journal-Report".Equals(text, StringComparison.OrdinalIgnoreCase) || "X-MS-Exchange-Moderation-Loop".Equals(text, StringComparison.OrdinalIgnoreCase) || "X-MS-Exchange-Generated-Message-Source".Equals(text, StringComparison.OrdinalIgnoreCase))
			{
				throw new RulesValidationException(TransportRulesStrings.CannotRemoveHeader(text));
			}
		}

		public override string Name
		{
			get
			{
				return "RemoveHeader";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return RemoveHeader.argumentTypes;
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
			string text = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			transportRulesEvaluationContext.MailItem.Message.MimeDocument.RootPart.Headers.RemoveAll(text);
			if (text.Equals("subject", StringComparison.OrdinalIgnoreCase))
			{
				transportRulesEvaluationContext.MailItem.Message.Subject = null;
			}
			return ExecutionControl.Execute;
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string)
		};
	}
}
