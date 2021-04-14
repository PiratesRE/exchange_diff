using System;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AddToRecipientSmtpOnly : AddRecipientAction
	{
		public AddToRecipientSmtpOnly(ShortList<Argument> arguments) : base(arguments, true)
		{
		}

		public override string Name
		{
			get
			{
				return "AddToRecipientSmtpOnly";
			}
		}

		public override RecipientP2Type RecipientP2Type
		{
			get
			{
				return RecipientP2Type.To;
			}
		}
	}
}
