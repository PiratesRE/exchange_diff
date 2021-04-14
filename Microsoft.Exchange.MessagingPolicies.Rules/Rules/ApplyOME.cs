using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class ApplyOME : SetHeaderUniqueValue
	{
		public ApplyOME(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Version MinimumVersion
		{
			get
			{
				return ApplyOME.CurrentVersion;
			}
		}

		public override string Name
		{
			get
			{
				return "ApplyOME";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return ApplyOME.ArgumentTypes;
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
