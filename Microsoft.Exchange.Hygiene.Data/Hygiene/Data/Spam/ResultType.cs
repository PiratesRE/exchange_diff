using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public enum ResultType : byte
	{
		AddScore,
		MarkAsSpam,
		SkipFiltering,
		AddFeatureScore,
		Reject,
		MarkAsHighRisk,
		SetPartition,
		SilentDrop,
		MarkAsBulk
	}
}
