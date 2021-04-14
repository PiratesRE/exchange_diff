using System;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class SetHeader : HeaderValueAction
	{
		public SetHeader(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "SetHeader";
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			string text = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			string text2 = (string)base.Arguments[1].GetValue(transportRulesEvaluationContext);
			MailItem mailItem = transportRulesEvaluationContext.MailItem;
			if (text.Equals("subject", StringComparison.OrdinalIgnoreCase))
			{
				transportRulesEvaluationContext.MailItem.Message.Subject = text2;
			}
			HeaderList headers = mailItem.Message.MimeDocument.RootPart.Headers;
			Header header = headers.FindFirst(text);
			if (header != null)
			{
				header.Value = text2;
			}
			else
			{
				TransportUtils.AddHeaderToMail(mailItem.Message, text, text2);
			}
			baseContext.ResetTextProcessingContext(text, true);
			return ExecutionControl.Execute;
		}
	}
}
