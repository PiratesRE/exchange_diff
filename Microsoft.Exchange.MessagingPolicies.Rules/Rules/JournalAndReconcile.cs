using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class JournalAndReconcile : JournalBase
	{
		public override Type[] ArgumentsType
		{
			get
			{
				return JournalAndReconcile.argumentTypes;
			}
		}

		public override string Name
		{
			get
			{
				return "JournalAndReconcile";
			}
		}

		public JournalAndReconcile(ShortList<Argument> args) : base(args)
		{
			string text = args[0].GetValue(null).ToString();
			string address = args[1].GetValue(null).ToString();
			if (text.Length < 1)
			{
				throw new RulesValidationException(TransportRulesStrings.InvalidReconciliationGuid(text));
			}
			int offset = 0;
			if (text[0] == '!')
			{
				offset = 1;
			}
			if (!JournalAndReconcile.ValidateGuid(text, offset))
			{
				throw new RulesValidationException(TransportRulesStrings.InvalidReconciliationGuid(text));
			}
			if (!SmtpAddress.IsValidSmtpAddress(address))
			{
				throw new RulesValidationException(TransportRulesStrings.InvalidAddress(address));
			}
		}

		protected override string MailItemProperty
		{
			get
			{
				return "Microsoft.Exchange.JournalReconciliationAccounts";
			}
		}

		protected override string GetItemToAdd(TransportRulesEvaluationContext context)
		{
			return string.Format("{0}{1}{2}", base.Arguments[0].GetValue(context), '+', base.Arguments[1].GetValue(context));
		}

		private static bool ValidateGuid(string text, int offset)
		{
			if (text.Length - offset != "00000000-0000-0000-0000-000000000000".Length)
			{
				return false;
			}
			for (int i = 0; i < "00000000-0000-0000-0000-000000000000".Length; i++)
			{
				if ("00000000-0000-0000-0000-000000000000"[i] == '0')
				{
					if (!Uri.IsHexDigit(text[i + offset]))
					{
						return false;
					}
				}
				else if (text[i + offset] != '-')
				{
					return false;
				}
			}
			return true;
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string),
			typeof(string)
		};
	}
}
