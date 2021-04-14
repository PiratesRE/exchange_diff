using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IHealthValidationResult : IHealthValidationResultMinimal
	{
		bool IsTargetCopyHealthy { get; }

		bool IsActiveCopyHealthy { get; }

		CopyStatusClientCachedEntry TargetCopyStatus { get; }

		CopyStatusClientCachedEntry ActiveCopyStatus { get; }
	}
}
