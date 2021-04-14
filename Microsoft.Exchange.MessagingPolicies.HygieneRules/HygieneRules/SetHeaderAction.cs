using System;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.HygieneRules
{
	internal class SetHeaderAction : Microsoft.Exchange.MessagingPolicies.Rules.Action
	{
		public SetHeaderAction(ShortList<Argument> arguments) : base(arguments)
		{
			if (!(base.Arguments[0] is Value) || !(base.Arguments[1] is Value))
			{
				throw new RulesValidationException(RulesStrings.ActionRequiresConstantArguments(this.Name));
			}
			string text = (string)base.Arguments[0].GetValue(null);
			string value = (string)base.Arguments[1].GetValue(null);
			if (!TransportUtils.IsHeaderValid(text))
			{
				throw new RulesValidationException(HygieneRulesStrings.InvalidHeaderName(text));
			}
			if (!TransportUtils.IsHeaderSettable(text, value))
			{
				throw new RulesValidationException(HygieneRulesStrings.CannotSetHeader(text, value));
			}
		}

		public override string Name
		{
			get
			{
				return "SetHeader";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return SetHeaderAction.argumentTypes;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			HygieneTransportRulesEvaluationContext hygieneTransportRulesEvaluationContext = (HygieneTransportRulesEvaluationContext)baseContext;
			string text = (string)base.Arguments[0].GetValue(hygieneTransportRulesEvaluationContext);
			string text2 = (string)base.Arguments[1].GetValue(hygieneTransportRulesEvaluationContext);
			MailItem mailItem = hygieneTransportRulesEvaluationContext.MailItem;
			if (text.Equals("subject", StringComparison.OrdinalIgnoreCase))
			{
				hygieneTransportRulesEvaluationContext.MailItem.Message.Subject = text2;
			}
			HeaderList headers = mailItem.Message.MimeDocument.RootPart.Headers;
			Header header = headers.FindFirst(text);
			if (header != null)
			{
				header.Value = text2;
				return ExecutionControl.Execute;
			}
			TransportUtils.AddHeaderToMail(mailItem.Message, text, text2);
			return ExecutionControl.Execute;
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string),
			typeof(string)
		};
	}
}
