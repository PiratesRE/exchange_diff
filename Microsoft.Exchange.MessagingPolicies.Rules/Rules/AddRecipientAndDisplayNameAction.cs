using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal abstract class AddRecipientAndDisplayNameAction : AddRecipientAction
	{
		protected AddRecipientAndDisplayNameAction(ShortList<Argument> arguments, bool isToRecipient) : base(arguments, isToRecipient)
		{
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return AddRecipientAndDisplayNameAction.displayNameAndAddressType;
			}
		}

		public override string GetDisplayName(RulesEvaluationContext baseContext)
		{
			return (string)base.Arguments[1].GetValue(baseContext);
		}

		private static Type[] displayNameAndAddressType = new Type[]
		{
			typeof(string),
			typeof(string)
		};
	}
}
