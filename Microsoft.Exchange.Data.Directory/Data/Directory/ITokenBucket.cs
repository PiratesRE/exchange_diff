using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal interface ITokenBucket
	{
		int PendingCharges { get; }

		DateTime? LockedUntilUtc { get; }

		bool Locked { get; }

		DateTime? LockedAt { get; }

		int MaximumBalance { get; }

		int MinimumBalance { get; }

		int RechargeRate { get; }

		float GetBalance();

		DateTime LastUpdateUtc { get; }

		void Increment();

		void Decrement(TimeSpan extraDuration = default(TimeSpan), bool reverseBudgetCharge = false);
	}
}
