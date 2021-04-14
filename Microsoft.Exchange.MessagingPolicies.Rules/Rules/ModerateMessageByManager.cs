using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class ModerateMessageByManager : ModerateMessageByUser
	{
		public ModerateMessageByManager(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "ModerateMessageByManager";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return ModerateMessageByManager.argumentTypes;
			}
		}

		protected override string GetModeratorList(RulesEvaluationContext baseContext)
		{
			return TransportUtils.GetSenderManagerAddress(baseContext);
		}

		private static Type[] argumentTypes = new Type[0];
	}
}
