using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal class DelayEnforcementResults
	{
		public DelayEnforcementResults(DelayInfo innerDelay, TimeSpan delayedAmount) : this(innerDelay, true, null, delayedAmount)
		{
		}

		public DelayEnforcementResults(DelayInfo innerDelay, string notEnforcedReason) : this(innerDelay, false, notEnforcedReason, TimeSpan.Zero)
		{
		}

		private DelayEnforcementResults(DelayInfo innerDelay, bool enforced, string notEnforcedReason, TimeSpan delayedAmount)
		{
			this.DelayInfo = innerDelay;
			this.Enforced = enforced;
			this.NotEnforcedReason = notEnforcedReason;
			this.DelayedAmount = delayedAmount;
		}

		public DelayInfo DelayInfo { get; private set; }

		public bool Enforced { get; private set; }

		public TimeSpan DelayedAmount { get; private set; }

		public string NotEnforcedReason { get; private set; }

		public const string NotEnforcedReasonMaxDelayedThreads = "Max Delayed Threads Exceeded";

		public const string NotEnforcedReasonNoDelay = "No Delay Necessary";

		public const string NotEnforcedReasonStrict = "Strict Delay Exceeds Preferred Delay";

		public const string NotEnforcedReasonCanceled = "OnBeforeDelay delegate returned false";

		public const string NotEnforcedReasonTooLong = "Delay Too Long";
	}
}
