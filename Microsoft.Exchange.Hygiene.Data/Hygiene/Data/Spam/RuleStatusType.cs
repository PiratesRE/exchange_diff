using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public enum RuleStatusType : byte
	{
		PreApproved,
		Approved,
		Publishing,
		Published,
		PublishingFailed,
		ValidationFailed,
		Submitted,
		PeerReviewFailed
	}
}
