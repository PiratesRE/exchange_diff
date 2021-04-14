using System;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal enum DiscardReason
	{
		ExplicitlyDiscarded,
		DeletedByAdmin,
		Resubmitted,
		DiscardAll,
		Expired
	}
}
