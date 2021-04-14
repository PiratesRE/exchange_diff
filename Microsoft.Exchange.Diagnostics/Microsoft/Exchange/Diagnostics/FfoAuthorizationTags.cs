using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct FfoAuthorizationTags
	{
		public const int FfoRunspace = 0;

		public const int PartnerConfig = 1;

		public const int FfoRps = 2;

		public const int FfoRpsBudget = 3;

		public const int FaultInjection = 4;

		public const int FfoServicePlans = 5;

		public static Guid guid = new Guid("2AEBD40A-8FA5-4159-A644-54F41B37D965");
	}
}
