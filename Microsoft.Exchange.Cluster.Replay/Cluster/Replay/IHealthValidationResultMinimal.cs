using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	public interface IHealthValidationResultMinimal
	{
		Guid IdentityGuid { get; }

		string Identity { get; }

		int HealthyCopiesCount { get; }

		int HealthyPassiveCopiesCount { get; }

		int TotalPassiveCopiesCount { get; }

		bool IsValidationSuccessful { get; }

		bool IsSiteValidationSuccessful { get; }

		bool IsAnyCachedCopyStatusStale { get; }

		string ErrorMessage { get; set; }

		string ErrorMessageWithoutFullStatus { get; set; }
	}
}
