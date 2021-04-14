using System;
using System.Collections.Generic;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class FixedThrottleEntry : ThrottleDescriptionEntry
	{
		public FixedThrottleEntry(RecoveryActionId recoveryActionId, string resourceName, int localMinimumMinutesBetweenAttempts, int localMaximumAllowedAttemptsInOneHour, int localMaximumAllowedAttemptsInADay, int groupMinimumMinutesBetweenAttempts, int groupMaximumAllowedAttemptsInADay) : base(ThrottleEntryType.BaseConfig, recoveryActionId, ResponderCategory.Default, "*", "*", resourceName)
		{
			this.ThrottleParameters = new ThrottleParameters(true, localMinimumMinutesBetweenAttempts, localMaximumAllowedAttemptsInOneHour, localMaximumAllowedAttemptsInADay, groupMinimumMinutesBetweenAttempts, groupMaximumAllowedAttemptsInADay);
		}

		public FixedThrottleEntry(RecoveryActionId recoveryActionId, int localMinimumMinutesBetweenAttempts, int localMaximumAllowedAttemptsInOneHour, int localMaximumAllowedAttemptsInADay, int groupMinimumMinutesBetweenAttempts, int groupMaximumAllowedAttemptsInADay) : base(ThrottleEntryType.BaseConfig, recoveryActionId, ResponderCategory.Default, "*", "*", "*")
		{
			this.ThrottleParameters = new ThrottleParameters(true, localMinimumMinutesBetweenAttempts, localMaximumAllowedAttemptsInOneHour, localMaximumAllowedAttemptsInADay, groupMinimumMinutesBetweenAttempts, groupMaximumAllowedAttemptsInADay);
		}

		public FixedThrottleEntry(RecoveryActionId recoveryActionId, ResponderCategory responderCategory, string responderTypeName, string responderName, string resourceName, ThrottleParameters throttleParameters) : base(ThrottleEntryType.Effective, recoveryActionId, responderCategory, responderTypeName, responderName, resourceName)
		{
			this.ThrottleParameters = throttleParameters.Clone();
		}

		public ThrottleParameters ThrottleParameters { get; private set; }

		internal override Dictionary<string, string> GetPropertyBag()
		{
			return this.ThrottleParameters.ToDictionary();
		}
	}
}
