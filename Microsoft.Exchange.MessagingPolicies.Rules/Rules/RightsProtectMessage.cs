using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class RightsProtectMessage : SetHeaderUniqueValue
	{
		public RightsProtectMessage(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "RightsProtectMessage";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return TransportRuleConstants.VersionedContainerBaseVersion;
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return RightsProtectMessage.ArgumentTypes;
			}
		}

		private static readonly Type[] ArgumentTypes = new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string)
		};
	}
}
