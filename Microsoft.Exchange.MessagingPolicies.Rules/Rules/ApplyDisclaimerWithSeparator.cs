using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class ApplyDisclaimerWithSeparator : ApplyDisclaimerWithSeparatorAndReadingOrder
	{
		public ApplyDisclaimerWithSeparator(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return ApplyDisclaimerWithSeparator.argumentTypes;
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return ApplyDisclaimerWithSeparator.minimumVersion;
			}
		}

		public override string Name
		{
			get
			{
				return "ApplyDisclaimerWithSeparator";
			}
		}

		protected override string GetReadingOrder(RulesEvaluationContext context)
		{
			return "LeftToRight";
		}

		private static Version minimumVersion = new Version("1.2");

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string)
		};
	}
}
