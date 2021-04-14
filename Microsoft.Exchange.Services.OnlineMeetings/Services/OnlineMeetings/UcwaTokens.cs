using System;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal static class UcwaTokens
	{
		internal static string Normalize(string token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			return token.ToLowerInvariant();
		}

		public const string Options = "scheduled/schedulingoptions";

		public const string Summaries = "scheduled/summaries";

		public const string DefaultValues = "onlinemeetingdefaultvalues";

		public const string MyOnlineMeetings = "myOnlineMeetings";

		public const string OnlineMeetingCustomization = "onlinemeetinginvitationcustomization";

		public const string OnlineMeetingPhoneDialIn = "phonedialininformation";

		public const string OnlineMeetingPolicies = "onlinemeetingpolicies";

		public const string OnlineMeetingEligibleValues = "onlinemeetingeligiblevalues";

		public const string AssignedMeeting = "myassignedonlinemeeting";
	}
}
