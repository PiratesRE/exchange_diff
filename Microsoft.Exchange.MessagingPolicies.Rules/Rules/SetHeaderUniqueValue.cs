using System;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class SetHeaderUniqueValue : HeaderValueAction
	{
		public SetHeaderUniqueValue(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "SetHeaderUniqueValue";
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			string text = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			string value = (string)base.Arguments[1].GetValue(transportRulesEvaluationContext);
			MailItem mailItem = transportRulesEvaluationContext.MailItem;
			Header[] array = mailItem.Message.MimeDocument.RootPart.Headers.FindAll(text);
			foreach (Header header in array)
			{
				if (header.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return ExecutionControl.Execute;
				}
			}
			TransportUtils.AddHeaderToMail(mailItem.Message, text, value);
			return ExecutionControl.Execute;
		}
	}
}
