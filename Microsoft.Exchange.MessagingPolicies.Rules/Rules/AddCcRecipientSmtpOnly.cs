using System;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AddCcRecipientSmtpOnly : AddRecipientAction
	{
		public AddCcRecipientSmtpOnly(ShortList<Argument> arguments) : base(arguments, false)
		{
		}

		public override string Name
		{
			get
			{
				return "AddCcRecipientSmtpOnly";
			}
		}

		public override RecipientP2Type RecipientP2Type
		{
			get
			{
				return RecipientP2Type.Cc;
			}
		}
	}
}
