using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class ExactUserComparer : IStringComparer
	{
		private ExactUserComparer()
		{
		}

		public static ExactUserComparer CreateInstance()
		{
			return ExactUserComparer.instance;
		}

		public bool Equals(string userX, string userY)
		{
			return !string.IsNullOrWhiteSpace(userX) && !string.IsNullOrWhiteSpace(userY) && string.Equals(userX, userY, StringComparison.InvariantCultureIgnoreCase);
		}

		private static readonly ExactUserComparer instance = new ExactUserComparer();
	}
}
