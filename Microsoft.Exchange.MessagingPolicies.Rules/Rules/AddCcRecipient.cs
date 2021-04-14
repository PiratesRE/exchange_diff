using System;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AddCcRecipient : AddRecipientAndDisplayNameAction
	{
		public AddCcRecipient(ShortList<Argument> arguments) : base(arguments, false)
		{
		}

		public override string Name
		{
			get
			{
				return "AddCcRecipient";
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
