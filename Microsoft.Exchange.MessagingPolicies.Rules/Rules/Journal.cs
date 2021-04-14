using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class Journal : JournalBase
	{
		public override string Name
		{
			get
			{
				return "Journal";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return Journal.argumentTypes;
			}
		}

		public Journal(ShortList<Argument> args) : base(args)
		{
			string address = args[0].GetValue(null).ToString();
			if (!SmtpAddress.IsValidSmtpAddress(address))
			{
				throw new RulesValidationException(TransportRulesStrings.InvalidAddress(address));
			}
		}

		protected override string MailItemProperty
		{
			get
			{
				return "Microsoft.Exchange.JournalTargetRecips";
			}
		}

		protected override string GetItemToAdd(TransportRulesEvaluationContext context)
		{
			return (string)base.Arguments[0].GetValue(context);
		}

		internal string GetTargetAddress()
		{
			return (string)base.Arguments[0].GetValue(null);
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string)
		};
	}
}
