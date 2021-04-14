using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class ApplyDisclaimer : ApplyDisclaimerWithSeparator
	{
		public ApplyDisclaimer(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return ApplyDisclaimer.argumentTypes;
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return TransportRuleConstants.VersionedContainerBaseVersion;
			}
		}

		public override string Name
		{
			get
			{
				return "ApplyDisclaimer";
			}
		}

		protected override string GetSeparator(RulesEvaluationContext context)
		{
			return "WithoutSeparator";
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string)
		};
	}
}
