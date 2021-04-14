using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class RemoveOME : SetHeaderUniqueValue
	{
		public RemoveOME(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Version MinimumVersion
		{
			get
			{
				return RemoveOME.CurrentVersion;
			}
		}

		public override string Name
		{
			get
			{
				return "RemoveOME";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return RemoveOME.ArgumentTypes;
			}
		}

		private static readonly Version CurrentVersion = new Version("15.00.0007.00");

		private static readonly Type[] ArgumentTypes = new Type[]
		{
			typeof(string),
			typeof(string)
		};
	}
}
